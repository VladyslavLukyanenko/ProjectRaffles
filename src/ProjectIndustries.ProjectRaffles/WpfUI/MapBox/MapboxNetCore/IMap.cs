// using System;
// using System.IO;
// using System.Threading.Tasks;
//
// namespace ProjectIndustries.ProjectRaffles.WpfUI.MapBox.MapboxNetCore
// {
//   public interface IMap
//   {
//     string AccessToken { get; set; }
//     object MapStyle { get; set; }
//     bool RemoveAttribution { get; set; }
//     MapboxNetCore.GeoLocation Center { get; set; }
//     double Zoom { get; set; }
//     double Pitch { get; set; }
//     double Bearing { get; set; }
//     bool IsReady { get; }
//
//     dynamic Invoke { get; }
//     dynamic SoftInvoke { get; }
//     dynamic LazyInvoke { get; }
//
//     object SoftExecute(string expression);
//     object Execute(string expression);
//     Task<object> ExecuteAsync(string expression);
//
//     void AddImage(string id, MemoryStream stream);
//     void AddImage(string id, string base64);
//
//     Point2D Project(GeoLocation location);
//     GeoLocation UnProject(Point2D point);
//   }
// }