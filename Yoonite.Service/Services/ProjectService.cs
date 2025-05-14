using System;
using System.Linq;
using Yoonite.Common.Models;
using Yoonite.Common.RequestAndResponses;
using Yoonite.Data;

namespace Yoonite.Service.Services
{
    public interface IProjectService
    {
        GetUserProjectsResponse GetUserProjects(GetUserProjectsRequest request);
        GetProjectsResponse GetProjects(GetProjectsRequest request);
        SaveProjectResponse SaveProject(SaveProjectRequest request);
        DeleteProjectResponse DeleteProjectAdmin(DeleteProjectRequest request);
        DeleteProjectResponse DeleteProject(DeleteProjectRequest request);
    }

    public class ProjectService : BaseService, IProjectService
    {
        public GetUserProjectsResponse GetUserProjects(GetUserProjectsRequest request)
        {
            try
            {
                var response = new GetUserProjectsResponse();
                using (var context = new YooniteEntities())
                {
                    var projects = context.Projects.Where(_ => _.AccountId.Equals(request.AccountId))
                                                   .Select(_ => new ProjectModel
                                                   {
                                                       Id = _.Id,
                                                       AccountId = _.AccountId,
                                                       ProjectName = _.ProjectName,
                                                       CompanyName = _.CompanyName,
                                                       Description = _.Description,
                                                       Url = _.Url,
                                                       City = _.City,
                                                       State = _.State,
                                                       ZipCode = _.ZipCode,
                                                       CanBeRemote = _.CanBeRemote,
                                                       IsActive = _.IsActive,
                                                       DateCreated = _.DateCreated,
                                                       ProjectSkills = _.ProjectSkillCrossLinks.Select(xl => new SkillModel
                                                       {
                                                           Id = xl.Skill.Id,
                                                           Name = xl.Skill.Name,
                                                           Description = xl.Skill.Description,
                                                           IsActive = xl.Skill.IsActive,
                                                           DateCreated = xl.Skill.DateCreated
                                                       }).ToList()
                                                   });
                    //response.Projects = MapperService.MapToList<Project, ProjectModel>(projects);
                    response.Projects = projects.ToList();
                    response.IsSuccess = true;
                    return response;
                }
            }
            catch (Exception ex)
            {
                LoggingService.LogError(new LogErrorRequest { ex = ex });
                return new GetUserProjectsResponse { ErrorMessage = "Unable to get user projects." };
            }
        }

        public GetProjectsResponse GetProjects(GetProjectsRequest request)
        {
            try
            {
                var response = new GetProjectsResponse();
                using (var context = new YooniteEntities())
                {
                    var projects = context.Projects.Where(_ => request.GetActiveAndInactive ? (_.IsActive || !_.IsActive) : _.IsActive)
                                                   .Select(_ => new ProjectModel
                                                   {
                                                       Id = _.Id,
                                                       AccountId = _.AccountId,
                                                       ProjectName = _.ProjectName,
                                                       CompanyName = _.CompanyName,
                                                       Description = _.Description,
                                                       Url = _.Url,
                                                       City = _.City,
                                                       State = _.State,
                                                       ZipCode = _.ZipCode,
                                                       CanBeRemote = _.CanBeRemote,
                                                       IsActive = _.IsActive,
                                                       DateCreated = _.DateCreated,
                                                       ProjectSkills = _.ProjectSkillCrossLinks.Select(xl => new SkillModel
                                                       {
                                                           Id = xl.Skill.Id,
                                                           Name = xl.Skill.Name,
                                                           Description = xl.Skill.Description,
                                                           IsActive = xl.Skill.IsActive,
                                                           DateCreated = xl.Skill.DateCreated
                                                       }).ToList()
                                                   });
                    //response.Projects = MapperService.MapToList<Project, ProjectModel>(projects);
                    response.Projects = projects.ToList();
                    response.IsSuccess = true;
                    return response;
                }
            }
            catch (Exception ex)
            {
                LoggingService.LogError(new LogErrorRequest { ex = ex });
                return new GetProjectsResponse { ErrorMessage = "Unable to get projects." };
            }
        }

        public SaveProjectResponse SaveProject(SaveProjectRequest request)
        {
            try
            {
                var response = new SaveProjectResponse();
                using (var context = new YooniteEntities())
                {
                    var project = context.Projects.FirstOrDefault(_ => _.Id.Equals(request.Project.Id));
                    if (project == null)
                    {
                        project = new Project { Id = Guid.NewGuid(), AccountId = request.Project.AccountId, DateCreated = DateTimeOffset.Now };
                        context.Projects.Add(project);
                    }

                    project.ProjectName = request.Project.ProjectName;
                    project.CompanyName = request.Project.CompanyName ?? "";
                    project.Description = request.Project.Description ?? "";
                    project.Url = request.Project.Url ?? "";
                    project.City = request.Project.City;
                    project.State = request.Project.State;
                    project.ZipCode = request.Project.ZipCode;
                    project.CanBeRemote = request.Project.CanBeRemote;
                    project.IsActive = request.Project.IsActive;

                    foreach (var skill in request.ProjectSkills)
                    {
                        var projectSkill = context.ProjectSkillCrossLinks.FirstOrDefault(_ => _.ProjectId.Equals(project.Id) && _.SkillId.Equals(skill));
                        if (projectSkill == null)
                        {
                            projectSkill = new ProjectSkillCrossLink { Id = Guid.NewGuid(), ProjectId = project.Id, SkillId = skill, DateCreated = DateTimeOffset.Now };
                            context.ProjectSkillCrossLinks.Add(projectSkill);
                        }
                    }

                    var removeProjectSkills = context.ProjectSkillCrossLinks.Where(_ => _.ProjectId.Equals(project.Id) && !request.ProjectSkills.Contains(_.SkillId));
                    context.ProjectSkillCrossLinks.RemoveRange(removeProjectSkills);
                    context.SaveChanges();

                    response.IsSuccess = true;
                    return response;
                }
            }
            catch (Exception ex)
            {
                LoggingService.LogError(new LogErrorRequest { ex = ex });
                return new SaveProjectResponse { ErrorMessage = "Unable to save project." };
            }
        }

        public DeleteProjectResponse DeleteProjectAdmin(DeleteProjectRequest request)
        {
            try
            {
                var response = new DeleteProjectResponse();
                using (var context = new YooniteEntities())
                {
                    var project = context.Projects.FirstOrDefault(_ => _.Id.Equals(request.ProjectId));
                    if (project == null) throw new ApplicationException($"Unable to find project id {request.ProjectId}");
                    var projectSkills = context.ProjectSkillCrossLinks.Where(_ => _.ProjectId.Equals(project.Id));
                    context.ProjectSkillCrossLinks.RemoveRange(projectSkills);
                    context.Projects.Remove(project);
                    context.SaveChanges();

                    response.IsSuccess = true;
                    return response;
                }
            }
            catch (Exception ex)
            {
                LoggingService.LogError(new LogErrorRequest { ex = ex });
                return new DeleteProjectResponse { ErrorMessage = "Unable to delete project." };
            }
        }

        public DeleteProjectResponse DeleteProject(DeleteProjectRequest request)
        {
            try
            {
                var response = new DeleteProjectResponse();
                using (var context = new YooniteEntities())
                {
                    var project = context.Projects.FirstOrDefault(_ => _.Id.Equals(request.ProjectId));
                    if (project == null) throw new ApplicationException($"Unable to find project id {request.ProjectId}");
                    project.IsDeleted = true;
                    project.IsActive = false;
                    context.SaveChanges();

                    response.IsSuccess = true;
                    return response;
                }
            }
            catch (Exception ex)
            {
                LoggingService.LogError(new LogErrorRequest { ex = ex });
                return new DeleteProjectResponse { ErrorMessage = "Unable to delete project" };
            }
        }
    }
}