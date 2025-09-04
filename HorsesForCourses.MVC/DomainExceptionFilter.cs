// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Mvc.Controllers;
// using Microsoft.AspNetCore.Mvc.Filters;
// using HorsesForCourses.Application.common;


// namespace HorsesForCourses.MVC
// {
//     public class DomainExceptionFilter : IExceptionFilter
//     {

//         public void OnException(ExceptionContext context)
//         {
//             if (context.Exception is DomainException ex)
//             {
//                 var controller = (Controller)context.Controller;

//                 controller.ModelState.AddModelError(string.Empty, ex.Message);

//                 // Return the same view with the current model
//                 context.Result = new ViewResult
//                 {
//                     ViewName = context.ActionDescriptor is ControllerActionDescriptor cad
//                         ? cad.ActionName // use the current action's view name
//                         : null,
//                     ViewData = controller.ViewData,
//                     TempData = controller.TempData
//                 };

//                 context.ExceptionHandled = true;
//             }
//         }
//     }
// }