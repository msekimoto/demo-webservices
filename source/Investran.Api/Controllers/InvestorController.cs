using System.Linq;
using System.Web.Http;
using Sungard.Investran.Suite.WebServices.Contracts.RelationshipManager.Investor;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sungard.Investran.Suite.WebServices.Contracts.Common.Udf;
using Sungard.Investran.Suite.WebServices.Contracts.RelationshipManager.Contact;
using Investran.Api.Auth;
using Investran.Api.Models;
using Sungard.Investran.Suite.WebServices.Contracts.Common.Lookups;
using Sungard.Investran.Suite.WebServices.Contracts.Common.TeamSecurity;
using Investran.Api.Helper;
using Investran.Api.Enums;
using Investran.Integration;

namespace Investran.Api.Controllers
{
    [RoutePrefix("api/investor")]
    public class InvestorController : ApiController
    {
        public Authentication Authentication { get; }
        public UdfHelper UdfHelper { get; }
        public EntityService EntityService { get; }
        public LookupService LookupService { get; }
        public SecurityService SecurityService { get; }

        public InvestorController(
            Authentication authentication, 
            UdfHelper udfHelper,
            EntityService entityService, 
            LookupService lookupService, 
            SecurityService securityService)
        {
            Authentication = authentication;
            UdfHelper = udfHelper;
            EntityService = entityService;
            LookupService = lookupService;
            SecurityService = securityService;
        }
     
        [HttpGet]
        [Route("{id:int}")]
        public async Task<IHttpActionResult> Get([FromUri]int id)
        {
            var investorDto = EntityService.Read<InvestorDto>(id);
            return Ok(investorDto);
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Post([FromBody]InvestorModel investorModel)
        {
            var investorDto = new InvestorDto
            {
                Name = investorModel.Name,
                NameAlias = investorModel.NameAlias,
                NoBridge = true,
                IncludeInGL = true,
                TaxId = investorModel.TaxId,
                InvestorClass = LookupService.Find<InvestorClassDto>(i => i.Id == investorModel.InvestorClassId).FirstOrDefault(),
                ReviewStatus = LookupService.Find<ReviewStatusDto>(i => i.Id == investorModel.ReviewStatusId).FirstOrDefault(),
                Domain = SecurityService.Find<DomainDto>(i => i.Id == investorModel.DomainId).FirstOrDefault(),
                EntityType = LookupService.Find<EntityTypeDto>(i => i.Id == investorModel.EntityTypeId).FirstOrDefault(),
                Udfs = new List<UdfValueDto>(),
                
            };

            if (investorModel.ContactIndividualId != null && investorModel.ContactIndividualId != 0)
                investorDto.Contact = EntityService.Read<IndividualDto>(investorModel.ContactIndividualId.Value);
            else if (investorModel.ContactOrganizationId != null && investorModel.ContactOrganizationId != 0)
                investorDto.Contact = EntityService.Read<OrganizationDto>(investorModel.ContactIndividualId.Value);

            foreach (var udf in investorModel.Udfs)
                UdfHelper.AddOrUpdateUdf(investorDto, EntityTypeEnum.Investor, udf.Id, udf.Value);

            var entity = EntityService.Create<InvestorDto>(investorDto);

            return Ok(entity);
        }
    }
}
