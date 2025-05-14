using System;
using System.Linq;
using Yoonite.Common.Models;
using Yoonite.Common.RequestAndResponses;
using Yoonite.Data;

namespace Yoonite.Service.Services
{
    public interface ISkillService
    {
        GetSkillsResponse GetSkills(GetSkillsRequest request);
        SaveSkillResponse SaveSkill(SaveSkillRequest request);
    }

    public class SkillService : BaseService, ISkillService
    {
        public GetSkillsResponse GetSkills(GetSkillsRequest request)
        {
            try
            {
                var response = new GetSkillsResponse();
                using (var context = new YooniteEntities())
                {
                    var skills = context.Skills.ToList();
                    response.Skills = MapperService.MapToList<Skill, SkillModel>(skills);
                    response.IsSuccess = true;
                    return response;
                }
            }
            catch (Exception ex)
            {
                LoggingService.LogError(new LogErrorRequest { ex = ex });
                return new GetSkillsResponse { ErrorMessage = "Unable to get skills." };
            }
        }

        public SaveSkillResponse SaveSkill(SaveSkillRequest request)
        {
            try
            {
                var response = new SaveSkillResponse();
                using (var context = new YooniteEntities())
                {
                    var skill = context.Skills.FirstOrDefault(_ => _.Id.Equals(request.Skill.Id));
                    if (skill == null)
                    {
                        skill = new Skill { Id = Guid.NewGuid(), DateCreated = DateTimeOffset.Now };
                        context.Skills.Add(skill);
                    }
                         
                    MapperService.Map(request.Skill, skill);
                    context.SaveChanges();

                    response.Skill = MapperService.Map<Skill, SkillModel>(skill);
                    response.IsSuccess = true;
                    return response;
                }
            }
            catch (Exception ex)
            {
                LoggingService.LogError(new LogErrorRequest { ex = ex });
                return new SaveSkillResponse { ErrorMessage = "Unable to save skill." };
            }
        }
    }
}