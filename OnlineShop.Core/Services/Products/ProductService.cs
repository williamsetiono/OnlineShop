using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using OnlineShop.Core.Models.Products;
using OnlineShop.Core.Provider;
using OnlineShop.Models.Entities;
using OnlineShop.Models.UnitOfWork;

namespace OnlineShop.Core.Services.Products
{
    public class ProductService : BaseService<Product>, IProductService
    {
        public ProductService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IQueryable<Product> GetAll()
        {
            return Repository.GetAll();
        }

        public Product FindById(int id)
        {
            return Repository.Get(id);
        }

        public void Insert(ProductViewModel model)
        {
            var entity = new Product()
            {
                CreatedDate = DateTime.Now,
                Description = model.Description,
                CategoryId = model.CategoryId == -1 ? null : model.CategoryId,
                Name = model.ProductName,
                Price = model.Price,
                Quantity = model.Quantity,
                Status = model.Status,
            };
            if (model.Images != null)
            {
                var imgs = SaveFiles(model.Images.ToList(), entity.Name);
                if (imgs.Any(a => a != null))
                {
                    SaveImgs(imgs, entity);
                }
                else
                {
                    SaveDefaultImg(entity);
                }
            }
            else
            {
                SaveDefaultImg(entity);
            }
            Repository.Insert(entity);
            UnitOfWork.Save();
        }

        public void Update(ProductViewModel model)
        {
            var entity = Repository.Get(model.Id);
            if(entity == null) throw new Exception("Data is not found");
            entity.CategoryId = model.CategoryId == -1 ? null : model.CategoryId;
            entity.Description = model.Description;
            entity.Name = model.ProductName;
            entity.Price = model.Price;
            entity.Quantity = model.Quantity;
            entity.Status = model.Status;
            if (model.Images != null)
            {
                var imgs = SaveFiles(model.Images.ToList(), entity.Name);
                if (imgs.Any(a => a != null))
                {
                    SaveImgs(imgs, entity);
                }
                else if(entity.Images != null && !entity.Images.Any())
                {
                    SaveDefaultImg(entity);
                }
            }
            else if (entity.Images != null && !entity.Images.Any())
            {
                SaveDefaultImg(entity);
            }
            Repository.Update(entity);
            UnitOfWork.Save(); 
        }

        public void Delete(int id)
        {
            var entity = Repository.Get(id);
            if(entity == null) throw new Exception("Data is not found");
            Repository.Delete(entity);
            UnitOfWork.Save();
        }
        #region Helper
        public ICollection<string> SaveFiles(List<HttpPostedFileBase> files,string productName)
        {
            var fileNames = new List<string>();
            productName = GenShortName(productName);
            var imageStore = Constants.ImageStore;
            if (files.Any() && files.All(a => a != null && !string.IsNullOrWhiteSpace(Path.GetExtension(a.FileName)) ))
            {
                foreach (var file in files)
                {
                    if (file != null)
                    {
                        var fileExtension = Path.GetExtension(file.FileName);
                        //rename file
                        var fileName = string.Format("{0}{1}{2}", productName, DateTime.Now.ToString("yyyyMMddhhmmss"), fileExtension);

                        // Save file to server
                        var uploadFolder = HttpContext.Current.Server.MapPath(imageStore);

                        if (!Directory.Exists(uploadFolder))
                            Directory.CreateDirectory(uploadFolder);

                        var pathImage = Path.Combine(uploadFolder, fileName);
                        file.SaveAs(pathImage);

                        fileNames.Add("/"+fileName);
                    }
                }
            }
            return fileNames;
        }
        public string GenShortName(string longName)
        {
            var ret = string.Empty;
            longName = longName.Replace(" ", "_");
            longName = longName.Replace(")", "");
            longName = longName.Replace("(", "");
            longName = longName.Replace("*", "");
            longName = longName.Replace("[", "");
            longName = longName.Replace("]", "");
            longName = longName.Replace("}", "");
            longName = longName.Replace("{", "");
            longName = longName.Replace(">", "");
            longName = longName.Replace("<", "");
            longName = longName.Replace("=", "");
            longName = longName.Replace(":", "");
            longName = longName.Replace(",", "");
            longName = longName.Replace("'", "");
            longName = longName.Replace("\"", "");
            longName = longName.Replace("/", "");
            longName = longName.Replace("\\", "");
            longName = longName.Replace("#", "");
            longName = longName.Replace("&", "");
            longName = longName.Replace("?", "");
            longName = longName.Replace(";", "");
            longName = longName.ToLower();

            return longName;
        }

        private void SaveImgs(IEnumerable<string> imgs,Product entity)
        {
            var images = imgs.Select(image => new Image()
            {
                Name = image,
                Product = entity,
                Status = true,
            }).ToList();
            images.First().Main = true;
            UnitOfWork.Repository<Image>().Insert(images);
        }

        private void SaveDefaultImg(Product entity)
        {
            string defaultImg = null;
            if (File.Exists(HttpContext.Current.Server.MapPath(Constants.DefaultImg)))
            {
                defaultImg = "default_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".png";
                var imgUrl = Constants.ImagePath + "/"+defaultImg;
                File.Copy(HttpContext.Current.Server.MapPath(Constants.DefaultImg), HttpContext.Current.Server.MapPath(imgUrl));
            }
            if (defaultImg != null)
            {
                var image = new Image()
                {
                    Name = "/"+defaultImg,
                    Product = entity,
                    Status = true,
                    Main = true
                };
                UnitOfWork.Repository<Image>().Insert(image);
            }
        }
        #endregion
    }
}
