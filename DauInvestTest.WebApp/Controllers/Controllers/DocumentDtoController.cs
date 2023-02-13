using AutoMapper;
using Common.Enums;
using Common.Interfaces;
using Common.Services.CodeService;
using Common.Services.DocumentWatermarkService;
using Common.Services.FileService;
using Common.Services.QrCodeService;
using DAL;
using DAL.Models;
using DataTransferObjects;
using DauInvestTest.WebApp.Models.RequestModels;
using DauInvestTest.WebApp.Models.ResponseModels;
using DocumentService.Services.FileService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace DauInvestTest.WebApp.Controllers.Controllers;

public class DocumentDtoController : BaseController<DocumentDto>
{
    private readonly IRepository<DocumentDto> _docsRepository;
    private readonly IRepository<CompanyDto> _companiesRepository;
    private readonly IRepository<SignOperationDto> _signOperationRepository;
    private readonly IFileService _fileService;
    private readonly ICodeService _codeService;
    private readonly IRepository<ConfirmationCodeDto> _codeRepository;

    private readonly QrCodeService _qrCodeService;
    private readonly DocumentWatermarkService _watermarkService;
    
    public DocumentDtoController(ILogger<DocumentDto> logger, UserManager<User> userManager, IConfiguration configuration, IMapper mapper, AppDbContext context, IRepository<DocumentDto> docsRepository, IRepository<CompanyDto> companiesRepository, IFileService fileService, IRepository<SignOperationDto> signOperationRepository, ICodeService codeService, QrCodeService qrCodeService, DocumentWatermarkService watermarkService, IRepository<ConfirmationCodeDto> codeRepository,IRepository<UserDto> userRepo) : base(logger, userManager, configuration, mapper, context,userRepo)
    {
        _docsRepository = docsRepository;
        _companiesRepository = companiesRepository;
        _fileService = fileService;
        _signOperationRepository = signOperationRepository;
        _codeService = codeService;
        _qrCodeService = qrCodeService;
        _watermarkService = watermarkService;
        _codeRepository = codeRepository;
    }
    
    [HttpGet("")]
    public async Task<IQueryable<DocumentDto>> Get(int top=10,int skip=0)
    {
        var user = await GetUserModelAsync();
        return _docsRepository.Get().Where(r => r.AuthorCompanyId == user.CompanyId).Skip(skip).Take(top);
    }
    
    [HttpGet("{key}")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    [Produces("application/pdf")]
    [AllowAnonymous]
    public async Task<IActionResult> Get(Guid key,[FromQuery] string accessToken=null)
    {
        var user = await GetUserModelAsync();
        
        var item = await GetDocumentAsync(key);

        if (item == null)
        {
            return NotFound();
        }
        
        if(user?.CompanyId==item.AuthorCompanyId||item.SignOperation.DocumentAccessToken==accessToken)

        {
            var file = await _fileService.GetFileAsync(item.Path);

            return File(file.GetBuffer(), "application/pdf", item.FileName);
        }

        return Unauthorized();
    }

    [HttpPost("")]
    public async Task<IActionResult> Post([FromForm] IFormFile file)
    {
        var user = await GetUserModelAsync();

        if (user.CompanyId == null)
        {
            return BadRequest("You have no company");
        }
        
        var company = await _companiesRepository.Get().FirstOrDefaultAsync(r => r.Id == user.CompanyId);

        if (company == null)
        {
            return NotFound("company not found");
        }

        var fileName = $"{Guid.NewGuid()}_{file.FileName}";
        
        var item = new DocumentDto()
        {
            AuthorCompanyId = user.CompanyId.Value,
            CreatedAt = DateTime.Now,
            FileName = fileName,
            Path = Folders.Documents + "/" + fileName
        };
        
        await using var stream = file.OpenReadStream();
        await _fileService.PutFileAsync(stream, Folders.Documents, item.FileName);

        await _docsRepository.CreateAsync(item);

        return NoContent();
    }
    
    [HttpDelete("{key}")]
    public async Task<IActionResult> Delete(Guid key)
    {
        var user = await GetUserModelAsync();
        var item = await GetDocumentAsync(key);
        
        if (item == null)
        {
            return NotFound();
        }

        if (item.AuthorCompanyId != user.CompanyId)
        {
            return Unauthorized();
        }

        _fileService.DeleteFile(item.Path);
        await _docsRepository.DeleteAsync(item);

        return NoContent();
    }

    [HttpPost("send-to-sign/{key}")]
    public async Task<IActionResult> SendToSign([FromRoute] Guid key, [FromBody] DocumentSignRequest request)
    {
        var user = await GetUserModelAsync();
        var item = await GetDocumentAsync(key);
        if (item == null)
        {
            return NotFound();
        }
        
        if (item.AuthorCompanyId != user.CompanyId)
        {
            return Unauthorized();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest();
        }
        
        var signOperation = _mapper.Map<SignOperationDto>(request);
        signOperation.Status = DocumentSigningStatus.SentToSign;
        signOperation.DocumentId = item.Id;
        signOperation.DocumentAccessToken = Guid.NewGuid().ToString("N");
        
        await _signOperationRepository.CreateAsync(signOperation);
        
        return Ok(new DocumentSentToSignResponse()
        {
            Url = getSignLink(item, signOperation)
        });
    }

    private string getSignLink(DocumentDto item, SignOperationDto signOperation)
    {
        return Url.ActionLink($"Get","DocumentDto",new { key = item.Id, accessToken=signOperation.DocumentAccessToken})!;
    }

    [AllowAnonymous]
    [HttpPost("get-code-for-sign/{token}")]
    public async Task<IActionResult> GetCodeForSign(string token)
    {
        var signOp = await _signOperationRepository.Get()
            .FirstOrDefaultAsync(r => r.DocumentAccessToken == token);

        if (signOp == null)
        {
            return NotFound();
        }

        Common.Services.CodeService.CodeGenerationResult? codeGen;
        ConfirmationCodeDto? codeEntity;
        switch (signOp.Status)
        {
            case DocumentSigningStatus.SentToSign:
                codeGen = await _codeService.GenerateCode(signOp.RecipientPhone);
                if (codeGen.Result != OperationResult.Success)
                {
                    return BadRequest(codeGen.Result);
                }
                _logger.LogInformation("generated code for sign as recepient: "+codeGen.Code);
                codeEntity = new ConfirmationCodeDto()
                {
                    CreatedAt = DateTime.UtcNow.AddHours(3),
                    SignOperationId = signOp.Id,
                    Code = codeGen.Code,
                    IsExpired = false
                };

                await _codeRepository.CreateAsync(codeEntity);
                break;
            case DocumentSigningStatus.RecipientSigned:
                var user = await GetUserModelAsync();
                if (user == null)
                {
                    return Unauthorized();
                }

                if (signOp.Document.AuthorCompanyId != user.CompanyId)
                {
                    return Unauthorized();
                }
                codeGen = await _codeService.GenerateCode(signOp.Document.AuthorCompany.ContactPhone);
                if (codeGen.Result != OperationResult.Success)
                {
                    return BadRequest(codeGen.Result);
                }

                codeEntity = new ConfirmationCodeDto()
                {
                    CreatedAt = DateTime.UtcNow.AddHours(3),
                    SignOperationId = signOp.Id,
                    Code = codeGen.Code,
                    IsExpired = false
                };

                await _codeRepository.CreateAsync(codeEntity);

                _logger.LogInformation("generated code for sign as company: "+codeGen.Code);
                
                break;
            case DocumentSigningStatus.BothSidesSigned:
                return BadRequest("document already signed");
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        return NoContent();
    }

    [AllowAnonymous]
    [HttpPost("sign/{token}/{code}")]
    public async Task<IActionResult> Sign(string token,string code)
    {
        var signOp = await _signOperationRepository.Get()
            .FirstOrDefaultAsync(r => r.DocumentAccessToken == token);

        if (signOp == null)
        {
            return NotFound();
        }

        var document = signOp.Document;
        ConfirmationCodeDto? correctCode;
        QrCodeGenerationResult? qrCode;
        MemoryStream? file;
        switch (signOp.Status)
        {
            case DocumentSigningStatus.SentToSign:
                correctCode = signOp.ConfirmationCodes.FirstOrDefault();
                if (correctCode == null)
                {
                    return NotFound("code not found or expired");
                }

                if (correctCode.Code != code)
                {
                    return BadRequest("code incorrect");
                }

                file = await _fileService.GetFileAsync(document.Path);

                file = _watermarkService.AddSignedByWatermark(file, signOp.RecipientData, WatermarkLocation.BottomOne);
                qrCode = _qrCodeService.GenerateQrCode(signOp.RecipientData);
                file = _watermarkService.AddQrCode(file, qrCode.QrCodeImageStream, qrCode.PixelSize.Item2,WatermarkLocation.BottomTwo);
                await _fileService.PutFileAsync(file, document.Path);

                signOp.Status++;
                correctCode.IsExpired = true;
                await _codeRepository.UpdateAsync(correctCode);
                await _signOperationRepository.UpdateAsync(signOp);
                
                break;
            case DocumentSigningStatus.RecipientSigned:
                var user = await GetUserModelAsync();
                if (user == null)
                {
                    return Unauthorized();
                }

                if (signOp.Document.AuthorCompanyId != user.CompanyId)
                {
                    return Unauthorized();
                }
                
                correctCode = signOp.ConfirmationCodes.FirstOrDefault();
                
                if (correctCode == null)
                {
                    return NotFound("code not found or expired");
                }

                if (correctCode.Code != code)
                {
                    return BadRequest("code incorrect");
                }
                
                file = await _fileService.GetFileAsync(document.Path);
                file = _watermarkService.AddSignedByWatermark(file, signOp.AuthorCompanyData, WatermarkLocation.BottomTree);
                qrCode = _qrCodeService.GenerateQrCode(signOp.AuthorCompanyData);
                file = _watermarkService.AddQrCode(file, qrCode.QrCodeImageStream, qrCode.PixelSize.Item2,WatermarkLocation.BottomFour);
                await _fileService.PutFileAsync(file, document.Path);
                
                signOp.Status++;
                await _signOperationRepository.UpdateAsync(signOp);
                correctCode.IsExpired = true;
                await _codeRepository.UpdateAsync(correctCode);
                break;
            case DocumentSigningStatus.BothSidesSigned:
                return BadRequest("document already signed");
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        return NoContent();
    }
    
    
    private async Task<DocumentDto> GetDocumentAsync(Guid key)
    {
        var item = await _docsRepository.Get().FirstOrDefaultAsync(r => r.Id == key);
        return item;
    }
}