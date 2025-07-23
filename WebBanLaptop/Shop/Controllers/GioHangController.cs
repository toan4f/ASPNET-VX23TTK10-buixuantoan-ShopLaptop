using Newtonsoft.Json.Linq;
using Shop.Areas.Administrator.Data.message;
using Shop.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Shop.Controllers
{
    public class GioHangController : Controller
    {
        MyDataDataContext data = new MyDataDataContext();

        public JsonResult GetbyID(int ID)
        {
            HomeModel home = new HomeModel();
            var Laptop = home.GetListChiTietDonHangTheoDonDatHang(ID).Find(x => x.madon.Equals(ID));
            return Json(Laptop, JsonRequestBehavior.AllowGet);
        }

        public List<GioHang> Laygiohang()// lấy ra danh sách sản phẩm trong giỏ hàng
        {
            List<GioHang> lstGiohang = Session["Giohang"] as List<GioHang>;
            if (lstGiohang == null) // nếu giỏ hàng bằng null thì khởi tạo giỏ hàng rồi gắn giỏ hàng cho Session["Giohang"]
            {
                lstGiohang = new List<GioHang>();
                Session["Giohang"] = lstGiohang;
            }
            return lstGiohang;
        }
        public ActionResult ThemGioHang(int id, string strURL)// thêm 1 sản phẩm vào giỏ hàng
        {
            List<GioHang> lstGioHang = Laygiohang();
            GioHang sanpham = lstGioHang.Find(n => n.malaptop == id); // tìm sản phẩm đã chọn theo id
            if (sanpham == null)
            {
                sanpham = new GioHang(id);
                lstGioHang.Add(sanpham);
                Notification.set_flash("Thêm giỏ hàng thành công!", "success");
                return Redirect(strURL);
            }
            else
            {
                sanpham.iSoluong++;
                return Redirect(strURL);
            }
        }
        private int TongSoLuong()// tính tổng số lượng đã mua trong giỏ hàng
        {
            int tsl = 0;
            List<GioHang> lstGiohang = Session["GioHang"] as List<GioHang>;
            if (lstGiohang != null)
            {
                tsl = lstGiohang.Sum(n => n.iSoluong);
            }
            return tsl;

        }
        private int TongSoLuongSanPham()// tính tổng số loại đã chọn mua
        {
            int tsl = 0;
            List<GioHang> lstGiohang = Session["GioHang"] as List<GioHang>;
            if (lstGiohang != null)
            {
                tsl = lstGiohang.Count;
            }
            return tsl;
        }
        private double TongTien()// tính tổng tiền giỏ hàng
        {
            double tt = 0;
            List<GioHang> lstGiohang = Session["GioHang"] as List<GioHang>;
            if (lstGiohang != null)
            {
                tt = lstGiohang.Sum(n => n.dThanhTien);
            }
            return tt;

        }
        public ActionResult GioHang()// request trả về View giỏ hàng
        {
            List<GioHang> lstGiohang = Laygiohang();
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            ViewBag.Tongsoluongsanpham = TongSoLuongSanPham();
            return View(lstGiohang);
        }
        public ActionResult GioHangPartial()// request trả về partialview giỏ hàng
        {
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            ViewBag.Tongsoluongsanpham = TongSoLuongSanPham();
            return PartialView();
        }
        public ActionResult XoaGiohang(int id)// xóa sản phẩm theo id
        {
            List<GioHang> lstGiohang = Laygiohang();
            GioHang sanpham = lstGiohang.SingleOrDefault(n => n.malaptop == id);
            if (sanpham != null)
            {
                lstGiohang.RemoveAll(n => n.malaptop == id);
                Notification.set_flash("Xóa mặt hàng thành công!", "success");
                return RedirectToAction("GioHang");
            }
            return RedirectToAction("GioHang");
        }
        public ActionResult CapnhatGiohang(int id, FormCollection collection)// cập nhật giỏ hàng theo id và form có số lượng
        {
            List<GioHang> lstGiohang = Laygiohang();
            GioHang sanpham = lstGiohang.SingleOrDefault(n => n.malaptop == id);
            try
            {
                if (sanpham != null)
                {               
                    if (int.Parse(collection["txtSolg"].ToString()) > 50)
                    {
                        Notification.set_flash("Mua hàng số lượng lớn > 50 liên hệ Admin!", "warning");
                        return RedirectToAction("GioHang");
                    }
                    else
                    {
                        sanpham.iSoluong = int.Parse(collection["txtSolg"].ToString());
                    }
                }
            }
            catch (Exception)
            {
                Notification.set_flash("Nhập sai định dạng số lượng!", "warning");
                return RedirectToAction("GioHang");
            }                                          
            
            return RedirectToAction("GioHang");
        }
        public ActionResult XoaTatCaGioHang()// xóa tất cả các mặt hàng trong giỏ hàng
        {
            /*List<GioHang> lstGioHang = Laygiohang();*/
            List<GioHang> lstGiohang = Laygiohang();
            lstGiohang.Clear();
            Notification.set_flash("Xóa giỏ hàng thành công!", "success");
            return RedirectToAction("GioHang");
        }

        [HttpGet]
        public ActionResult DatHang()// đặt hàng
        {
            if (Session["TaiKhoan"] == null || Session["TaiKhoan"].ToString() == "")
            {
                ViewBag.thongbao = "Bạn phải đăng nhập tài khoản khách mua hàng!";
                return RedirectToAction("Login", "Account");
            }
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            List<GioHang> lstGiohang = Laygiohang();
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            ViewBag.Tongsoluongsanpham = TongSoLuongSanPham();
            return View(lstGiohang);
        }
        [HttpPost]
        public ActionResult DatHang(FormCollection collection)
        {
            DonHang dh = new DonHang();
            AspNetUser kh = (AspNetUser)Session["TaiKhoan"];// ép session về kh để lấy thông tin
            Laptop lap = new Laptop(); // lấy
            List<GioHang> gh = Laygiohang();// lấy giỏ hàng
            //var ngaygiao = String.Format("{0:MM/dd/yyyy}", collection["NgayGiao"]);//lấy ngày giao format lại

            dh.makh = kh.Id;
            dh.ngaydat = DateTime.Now;
            //dh.ngaygiao = DateTime.Now;
            dh.giaohang = null;
            dh.thanhtoan = false;
            dh.tinhtrang = '0';
            /*if ((bool)Session["thanhtoan"] == true)
            {
                dh.thanhtoan = true;
            }
            else
            {
                dh.thanhtoan = false;
            }*/


            data.DonHangs.InsertOnSubmit(dh);
            data.SubmitChanges();
            try
            {
                foreach (var item in gh)
                {
                    ChiTietDonHang ctdh = new ChiTietDonHang();
                    ctdh.madon = dh.madon;
                    ctdh.malaptop = item.malaptop;
                    ctdh.soluong = item.iSoluong;
                    ctdh.dongia = (decimal)item.giaban;
                    data.ChiTietDonHangs.InsertOnSubmit(ctdh);
                    // lấy số lượng tồn trừ đi
                    lap = data.Laptops.FirstOrDefault(n => n.malaptop == item.malaptop);
                    if(lap.soluongton > ctdh.soluong && lap.soluongton != null)
                    {
                        lap.soluongton = lap.soluongton - ctdh.soluong;
                    }               
                    data.SubmitChanges();
                }

                data.SubmitChanges();
            }
            catch (Exception)
            {
                Notification.set_flash("Lỗi cập nhật dữ liệu!", "danger");
                return RedirectToAction("Index", "Home");
            }
            
            

            //Gửi mail tới khác dùng

            /*string detail = "";

            foreach (var item in gh)
            {
                detail += "Tài khoản:  " + kh.Email.ToString() + "------" + "Mật khẩu:  " + kh.PasswordHash + "=======================";
            }*/

            string content = System.IO.File.ReadAllText(Server.MapPath("~/Content/template/neworder.html"));

            //var total = gh.Sum(n => n.giaban);
            var total = TongTien();
            content = content.Replace("{CustomerName}", kh.hoten);
            content = content.Replace("{Phone}", kh.PhoneNumber);
            content = content.Replace("{Email}", kh.Email);
            content = content.Replace("{Total}", total.ToString());
            
            Notification.set_flash("Bạn đã đặt hàng thành công!", "success");
            Session["GioHang"] = null;
            return RedirectToAction("XacnhanDonhang", "GioHang");
        }
        public ActionResult XacnhanDonhang()//xác nhận đơn mạng
        {
            return View();
        }

        public ActionResult XacnhanThanhToan_MoMo()//xác nhận đơn mạng
        {
            return View();
        }

        public ActionResult ThanhToanThatBai()// Trả về View thông báo Thanh toán Thất bại
        {
            return View();
        }

        public ActionResult ConfirmPaymentClient()
        {
            return View();
        }

        //[HttpPost]
        public void SavePayment()// Lưu đơn hàng vào database
        {
            //cập nhật dữ liệu vào db

            DonHang dh = new DonHang();
            AspNetUser kh = (AspNetUser)Session["TaiKhoan"];// ép session về kh để lấy thông tin
            Laptop s = new Laptop();
            List<GioHang> gh = Laygiohang();
            //List<GioHang> gh = (List<GioHang>) Session["GioHang"];// lấy giỏ hàng

            dh.makh = kh.Id;
            dh.ngaydat = DateTime.Now;
            //dh.ngaygiao = DateTime.Now;
            dh.giaohang = null;
            dh.thanhtoan = true;
            dh.tinhtrang = '0';

            data.DonHangs.InsertOnSubmit(dh);
            data.SubmitChanges();
            foreach (var item in gh)
            {
                ChiTietDonHang ctdh = new ChiTietDonHang();
                ctdh.madon = dh.madon;
                ctdh.malaptop = item.malaptop;
                ctdh.soluong = item.iSoluong;
                ctdh.dongia = (decimal)item.giaban;
                s = data.Laptops.Single(n => n.malaptop == item.malaptop);
                data.SubmitChanges();
                data.ChiTietDonHangs.InsertOnSubmit(ctdh);
            }

            data.SubmitChanges();
            Notification.set_flash("Bạn đã thanh toán thành công!", "success");


            string content = System.IO.File.ReadAllText(Server.MapPath("~/Content/template/neworder.html"));

            //var total = gh.Sum(n => n.giaban);
            var total = TongTien();
            content = content.Replace("{CustomerName}", kh.hoten);
            content = content.Replace("{Phone}", kh.PhoneNumber);
            content = content.Replace("{Email}", kh.Email);
            content = content.Replace("{Total}", total.ToString(""));
            Session["GioHang"] = null;
        }

    }
}