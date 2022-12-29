using System.ComponentModel;
namespace Storeage.Models
{
    public class ImageModel
    {
        public string Name { get; set; }
        public int EmpID { get; set; }
        public string Project { get; set; }
        [DisplayName ("Upload Image")]
        public string? ImageDetails { get; set; }
        public IFormFile? File { get; set; }
    }
}
