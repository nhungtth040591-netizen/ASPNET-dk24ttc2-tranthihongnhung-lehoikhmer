using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using KhmerFestival.Web.Models;

namespace KhmerFestival.Web.Data
{
    public static class SeedData
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Đảm bảo database được tạo và migrate
            context.Database.Migrate();

            // Nếu đã có Roles thì coi như đã seed rồi → thoát
            if (context.Roles.Any())
                return;

            // ===== Roles =====
            var roles = new List<Role>
            {
                new Role
                {
                    Name = "Admin",
                    Description = "Quản trị hệ thống, toàn quyền thao tác.",
                    CreatedAtUtc = DateTime.UtcNow
                },
                new Role
                {
                    Name = "Editor",
                    Description = "Biên tập nội dung, quản lý bài viết, lễ hội.",
                    CreatedAtUtc = DateTime.UtcNow
                },
                new Role
                {
                    Name = "User",
                    Description = "Người dùng thường, có thể bình luận bài viết.",
                    CreatedAtUtc = DateTime.UtcNow
                }
            };
            context.Roles.AddRange(roles);
            context.SaveChanges();

            // ===== SystemConfigs =====
            if (!context.SystemConfigs.Any())
            {
                var configs = new List<SystemConfig>
                {
                    new SystemConfig
                    {
                        ConfigKey = "SiteName",
                        ConfigValue = "Website Lễ Hội Khmer",
                        Description = "Tên website hiển thị ở header, title."
                    },
                    new SystemConfig
                    {
                        ConfigKey = "ContactEmail",
                        ConfigValue = "contact@khmerfestival.local",
                        Description = "Email nhận liên hệ từ form Contact."
                    },
                    new SystemConfig
                    {
                        ConfigKey = "FeaturedFestivalCount",
                        ConfigValue = "3",
                        Description = "Số lễ hội nổi bật hiển thị trên trang chủ."
                    }
                };

                context.SystemConfigs.AddRange(configs);
                context.SaveChanges();
            }

            // ===== Tài khoản Admin (mật khẩu: Admin@123) =====
            var adminPasswordHash = HashPassword("Admin@123");
            var adminAccount = new Account
            {
                Email = "admin@khmerfestival.local",
                PasswordHash = adminPasswordHash.hash,
                PasswordSalt = adminPasswordHash.salt,
                FullName = "Quản trị viên hệ thống",
                Phone = "0123456789",
                IsActive = true,
                CreatedAtUtc = DateTime.UtcNow
            };
            context.Accounts.Add(adminAccount);
            context.SaveChanges();

            // ===== Tài khoản Editor (mật khẩu: Editor@123) =====
            var editorPasswordHash = HashPassword("Editor@123");
            var editorAccount = new Account
            {
                Email = "editor@khmerfestival.local",
                PasswordHash = editorPasswordHash.hash,
                PasswordSalt = editorPasswordHash.salt,
                FullName = "Biên tập viên lễ hội",
                Phone = "0987654321",
                IsActive = true,
                CreatedAtUtc = DateTime.UtcNow
            };
            context.Accounts.Add(editorAccount);
            context.SaveChanges();

            // Gán role cho các tài khoản
            var adminRole = context.Roles.First(r => r.Name == "Admin");
            var editorRole = context.Roles.First(r => r.Name == "Editor");

            context.AccountRoles.Add(new AccountRole
            {
                AccountId = adminAccount.AccountId,
                RoleId = adminRole.RoleId,
                GrantedAtUtc = DateTime.UtcNow
            });

            context.AccountRoles.Add(new AccountRole
            {
                AccountId = editorAccount.AccountId,
                RoleId = editorRole.RoleId,
                GrantedAtUtc = DateTime.UtcNow
            });

            context.SaveChanges();

            // ===== Locations (10 tỉnh/thành có đông người Khmer) =====
            var locations = new List<Location>
            {
                new Location
                {
                    Name = "Tỉnh Sóc Trăng",
                    ParentId = null,
                    Level = 1,
                    Notes = "Khu vực có nhiều lễ hội người Khmer, nổi tiếng với đua ghe Ngo.",
                    CreatedAtUtc = DateTime.UtcNow
                },
                new Location
                {
                    Name = "Tỉnh Trà Vinh",
                    ParentId = null,
                    Level = 1,
                    Notes = "Nhiều chùa Khmer và các lễ hội truyền thống lớn.",
                    CreatedAtUtc = DateTime.UtcNow
                },
                new Location
                {
                    Name = "Tỉnh An Giang",
                    ParentId = null,
                    Level = 1,
                    Notes = "Vùng Bảy Núi, có lễ hội đua bò và nhiều nghi lễ nông nghiệp.",
                    CreatedAtUtc = DateTime.UtcNow
                },
                new Location
                {
                    Name = "Tỉnh Kiên Giang",
                    ParentId = null,
                    Level = 1,
                    Notes = "Khu vực ven biển, có cộng đồng Khmer sinh sống lâu đời.",
                    CreatedAtUtc = DateTime.UtcNow
                },
                new Location
                {
                    Name = "Tỉnh Bạc Liêu",
                    ParentId = null,
                    Level = 1,
                    Notes = "Nhiều chùa Khmer, gắn với lễ hội Phật giáo Nam tông.",
                    CreatedAtUtc = DateTime.UtcNow
                },
                new Location
                {
                    Name = "Tỉnh Cà Mau",
                    ParentId = null,
                    Level = 1,
                    Notes = "Cộng đồng Khmer sống xen kẽ với người Kinh và Hoa.",
                    CreatedAtUtc = DateTime.UtcNow
                },
                new Location
                {
                    Name = "Tỉnh Vĩnh Long",
                    ParentId = null,
                    Level = 1,
                    Notes = "Vùng sông nước, có một số phum sóc Khmer.",
                    CreatedAtUtc = DateTime.UtcNow
                },
                new Location
                {
                    Name = "Tỉnh Hậu Giang",
                    ParentId = null,
                    Level = 1,
                    Notes = "Có các nghi lễ nông nghiệp truyền thống của người Khmer.",
                    CreatedAtUtc = DateTime.UtcNow
                },
                new Location
                {
                    Name = "Thành phố Cần Thơ",
                    ParentId = null,
                    Level = 1,
                    Notes = "Đô thị trung tâm ĐBSCL, có nhiều hoạt động giao lưu văn hóa Khmer.",
                    CreatedAtUtc = DateTime.UtcNow
                },
                new Location
                {
                    Name = "Tỉnh Tây Ninh",
                    ParentId = null,
                    Level = 1,
                    Notes = "Có cộng đồng Khmer sinh sống dọc biên giới.",
                    CreatedAtUtc = DateTime.UtcNow
                }
            };
            context.Locations.AddRange(locations);
            context.SaveChanges();

            var socTrang = locations.First(l => l.Name == "Tỉnh Sóc Trăng");
            var traVinh = locations.First(l => l.Name == "Tỉnh Trà Vinh");
            var anGiang = locations.First(l => l.Name == "Tỉnh An Giang");
            var kienGiang = locations.First(l => l.Name == "Tỉnh Kiên Giang");
            var bacLieu = locations.First(l => l.Name == "Tỉnh Bạc Liêu");
            var caMau = locations.First(l => l.Name == "Tỉnh Cà Mau");
            var vinhLong = locations.First(l => l.Name == "Tỉnh Vĩnh Long");
            var hauGiang = locations.First(l => l.Name == "Tỉnh Hậu Giang");
            var canTho = locations.First(l => l.Name == "Thành phố Cần Thơ");
            var tayNinh = locations.First(l => l.Name == "Tỉnh Tây Ninh");

            // ===== Categories (5 danh mục bài viết) =====
            var categories = new List<Category>
            {
                new Category
                {
                    Name = "Tin tức lễ hội",
                    Slug = "tin-tuc-le-hoi",
                    ParentCategoryId = null,
                    Description = "Cập nhật tin tức, thông báo liên quan đến các lễ hội Khmer.",
                    CreatedAtUtc = DateTime.UtcNow
                },
                new Category
                {
                    Name = "Văn hóa Khmer",
                    Slug = "van-hoa-khmer",
                    ParentCategoryId = null,
                    Description = "Giới thiệu phong tục, tập quán, đời sống văn hóa của người Khmer.",
                    CreatedAtUtc = DateTime.UtcNow
                },
                new Category
                {
                    Name = "Kinh nghiệm du lịch",
                    Slug = "kinh-nghiem-du-lich",
                    ParentCategoryId = null,
                    Description = "Hướng dẫn tham quan, ăn uống, di chuyển khi đi lễ hội.",
                    CreatedAtUtc = DateTime.UtcNow
                },
                new Category
                {
                    Name = "Ẩm thực lễ hội",
                    Slug = "am-thuc-le-hoi",
                    ParentCategoryId = null,
                    Description = "Món ăn, thức uống đặc trưng trong các dịp lễ hội Khmer.",
                    CreatedAtUtc = DateTime.UtcNow
                },
                new Category
                {
                    Name = "Nghệ thuật trình diễn",
                    Slug = "nghe-thuat-trinh-dien",
                    ParentCategoryId = null,
                    Description = "Múa, hát, nhạc ngũ âm, dù kê, rô băm... trong lễ hội.",
                    CreatedAtUtc = DateTime.UtcNow
                }
            };
            context.Categories.AddRange(categories);
            context.SaveChanges();

            var categoryNews = categories.First(c => c.Slug == "tin-tuc-le-hoi");
            var categoryCulture = categories.First(c => c.Slug == "van-hoa-khmer");
            var categoryTravel = categories.First(c => c.Slug == "kinh-nghiem-du-lich");
            var categoryFood = categories.First(c => c.Slug == "am-thuc-le-hoi");
            var categoryArt = categories.First(c => c.Slug == "nghe-thuat-trinh-dien");

            // ===== Festivals (10 lễ hội tiêu biểu) =====
            var year = DateTime.UtcNow.Year;

            var festivals = new List<Festival>
            {
                new Festival
                {
                    Name = "Chol Chnam Thmay",
                    Slug = "chol-chnam-thmay",
                    ShortDescription = "Tết cổ truyền mừng năm mới của người Khmer, diễn ra vào trung tuần tháng 4 dương lịch.",
                    Content = "Chol Chnam Thmay là dịp người Khmer dọn dẹp nhà cửa, mặc quần áo mới, đi chùa tắm Phật, đắp núi cát và tham gia nhiều trò chơi dân gian để đón năm mới.",
                    StartDate = new DateTime(year, 4, 13),
                    EndDate = new DateTime(year, 4, 15),
                    LocationId = socTrang.LocationId,
                    Meaning = "Mừng năm mới, cầu cho mưa thuận gió hòa, mùa màng bội thu và gia đình đoàn tụ.",
                    IsFeatured = true,
                    Status = 1,
                    CreatedAtUtc = DateTime.UtcNow
                },
                new Festival
                {
                    Name = "Pithi Sene Dolta",
                    Slug = "pithi-sene-dolta",
                    ShortDescription = "Lễ cúng ông bà tổ tiên, thể hiện lòng hiếu kính và tri ân người đã khuất.",
                    Content = "Trong lễ Sene Dolta, người Khmer dọn dẹp nhà cửa, chuẩn bị lễ vật dâng cúng, mời linh hồn ông bà về sum họp, sau đó đưa ông bà lên chùa nghe kinh và làm lễ tiễn vào ngày cuối.",
                    StartDate = new DateTime(year, 9, 10),
                    EndDate = new DateTime(year, 9, 12),
                    LocationId = anGiang.LocationId,
                    Meaning = "Tưởng nhớ tổ tiên, báo hiếu cha mẹ, cầu phúc cho người sống và người đã khuất.",
                    IsFeatured = true,
                    Status = 1,
                    CreatedAtUtc = DateTime.UtcNow
                },
                new Festival
                {
                    Name = "Ok Om Bok",
                    Slug = "ok-om-bok",
                    ShortDescription = "Lễ hội cúng Trăng và đua ghe Ngo, tổ chức sau vụ mùa để tạ ơn thần Mặt Trăng.",
                    Content = "Ok Om Bok là dịp người Khmer dâng cốm dẹp và hoa trái để tạ ơn thần Mặt Trăng, kết hợp với hội đua ghe Ngo, thả đèn nước và đèn gió tạo nên không khí rất sôi động.",
                    StartDate = new DateTime(year, 11, 1),
                    EndDate = new DateTime(year, 11, 1),
                    LocationId = socTrang.LocationId,
                    Meaning = "Tạ ơn thần Mặt Trăng, cầu cho mùa màng bội thu và cuộc sống ấm no.",
                    IsFeatured = true,
                    Status = 1,
                    CreatedAtUtc = DateTime.UtcNow
                },
                new Festival
                {
                    Name = "Lễ Chôl Vôsa (Nhập hạ)",
                    Slug = "chol-vosa",
                    ShortDescription = "Nghi lễ nhập hạ của chư tăng, đồng thời là dịp người dân cầu an, cầu mưa thuận gió hòa.",
                    Content = "Trong lễ Chôl Vôsa, người Khmer mang lễ vật đến chùa, thắp nến và dâng cúng các nhu yếu phẩm cho sư sãi trong ba tháng an cư kiết hạ.",
                    StartDate = new DateTime(year, 7, 15),
                    EndDate = new DateTime(year, 7, 15),
                    LocationId = hauGiang.LocationId,
                    Meaning = "Cầu an cho gia đình và cộng đồng, thể hiện sự hộ trì tăng đoàn.",
                    IsFeatured = false,
                    Status = 1,
                    CreatedAtUtc = DateTime.UtcNow
                },
                new Festival
                {
                    Name = "Lễ Bon Phnôm Pôn (Lễ Ngàn Núi)",
                    Slug = "bon-phnom-pon",
                    ShortDescription = "Lễ đắp núi cát để tạ lỗi với các sinh vật đã bị giết làm thức ăn.",
                    Content = "Đồng bào Khmer cùng nhau đắp các ngọn núi cát tượng trưng, dâng lễ và nghe kinh để tích phước, xin các loài vật tha thứ, mong tránh nghiệp báo sau khi qua đời.",
                    StartDate = new DateTime(year, 3, 10),
                    EndDate = new DateTime(year, 3, 12),
                    LocationId = kienGiang.LocationId,
                    Meaning = "Tạ lỗi với muôn loài, tích lũy công đức và cầu an cho cộng đồng.",
                    IsFeatured = false,
                    Status = 1,
                    CreatedAtUtc = DateTime.UtcNow
                },
                new Festival
                {
                    Name = "Lễ Bon Kâm San Srok (Lễ cầu an, hội làng)",
                    Slug = "bon-kam-san-srok",
                    ShortDescription = "Lễ cầu an cho phum sóc sau Tết Chol Chnam Thmay, kết hợp múa hát và trò chơi dân gian.",
                    Content = "Người Khmer thỉnh sư sãi đi sớt bát, đọc kinh cầu an, cúng các vị thần bảo vệ đất đai và tổ chức thả đèn gió, văn nghệ, trò chơi dân gian để mừng thành quả lao động.",
                    StartDate = new DateTime(year, 4, 25),
                    EndDate = new DateTime(year, 4, 26),
                    LocationId = traVinh.LocationId,
                    Meaning = "Cầu cho mưa thuận gió hòa, mùa màng bội thu và phum sóc yên vui.",
                    IsFeatured = false,
                    Status = 1,
                    CreatedAtUtc = DateTime.UtcNow
                },
                new Festival
                {
                    Name = "Lễ Dâng Y Kathina",
                    Slug = "dang-y-kathina",
                    ShortDescription = "Lễ dâng y cà sa và vật dụng cho chư tăng sau ba tháng an cư kiết hạ.",
                    Content = "Phật tử Khmer rước đoàn lễ, dâng y cà sa cùng nhiều vật phẩm đến chùa, nhiễu Phật và cử hành nghi thức dâng Y Kathina trang trọng, tạo nên không khí lễ hội vui tươi.",
                    StartDate = new DateTime(year, 10, 20),
                    EndDate = new DateTime(year, 10, 21),
                    LocationId = bacLieu.LocationId,
                    Meaning = "Tôn kính chư tăng, cầu an cho gia đình và phum sóc, phát huy truyền thống bố thí cúng dường.",
                    IsFeatured = true,
                    Status = 1,
                    CreatedAtUtc = DateTime.UtcNow
                },
                new Festival
                {
                    Name = "Lễ Miakha Bôchia",
                    Slug = "miakha-bochia",
                    ShortDescription = "Lễ tưởng nhớ các sự kiện quan trọng trong Phật giáo, tổ chức ngày rằm tháng Giêng.",
                    Content = "Trong lễ Miakha Bôchia, chư tăng và Phật tử tập trung về chùa tụng kinh, thắp nến và dâng lễ để tưởng nhớ Đức Phật và những sự kiện trọng đại trong lịch sử Phật giáo.",
                    StartDate = new DateTime(year, 2, 15),
                    EndDate = new DateTime(year, 2, 15),
                    LocationId = vinhLong.LocationId,
                    Meaning = "Thể hiện lòng tôn kính đối với Tam Bảo và cầu bình an cho gia đình, cộng đồng.",
                    IsFeatured = false,
                    Status = 1,
                    CreatedAtUtc = DateTime.UtcNow
                },
                new Festival
                {
                    Name = "Lễ hội Đom Lơng Néak Tà",
                    Slug = "dom-long-neak-ta",
                    ShortDescription = "Lễ hội truyền thống cúng Néak Tà, cầu an và cầu mưa thuận gió hòa.",
                    Content = "Tại các miếu Néak Tà, người Khmer tổ chức cúng bái, dâng lễ và tham gia nhiều hoạt động văn hóa, thể thao dân gian như nhạc ngũ âm, dù kê, rô băm, kéo co, đua ghe...",
                    StartDate = new DateTime(year, 5, 5),
                    EndDate = new DateTime(year, 5, 6),
                    LocationId = traVinh.LocationId,
                    Meaning = "Cầu bình an, sức khỏe cho người dân trong phum sóc, tăng cường gắn kết cộng đồng.",
                    IsFeatured = true,
                    Status = 1,
                    CreatedAtUtc = DateTime.UtcNow
                },
                new Festival
                {
                    Name = "Lễ hội đua bò Bảy Núi",
                    Slug = "dua-bo-bay-nui",
                    ShortDescription = "Lễ hội đua bò truyền thống vùng Bảy Núi An Giang, gắn với lễ Dolta của người Khmer.",
                    Content = "Các đôi bò được trang trí đẹp mắt, do nông dân điều khiển tranh tài trên ruộng bùn. Lễ hội thu hút đông đảo người dân và du khách đến cổ vũ mỗi năm.",
                    StartDate = new DateTime(year, 10, 1),
                    EndDate = new DateTime(year, 10, 1),
                    LocationId = anGiang.LocationId,
                    Meaning = "Tạ ơn bò và người chăm sóc sau vụ mùa, đồng thời tạo sân chơi gắn kết cộng đồng vùng Bảy Núi.",
                    IsFeatured = true,
                    Status = 1,
                    CreatedAtUtc = DateTime.UtcNow
                }
            };

            context.Festivals.AddRange(festivals);
            context.SaveChanges();

            var cholChnamThmay = festivals.First(f => f.Slug == "chol-chnam-thmay");
            var seneDolta = festivals.First(f => f.Slug == "pithi-sene-dolta");
            var okOmBok = festivals.First(f => f.Slug == "ok-om-bok");
            var cholVosa = festivals.First(f => f.Slug == "chol-vosa");
            var bonPhnomPon = festivals.First(f => f.Slug == "bon-phnom-pon");
            var bonKamSanSrok = festivals.First(f => f.Slug == "bon-kam-san-srok");
            var dangYKathina = festivals.First(f => f.Slug == "dang-y-kathina");
            var miakhaBochia = festivals.First(f => f.Slug == "miakha-bochia");
            var domLongNeakTa = festivals.First(f => f.Slug == "dom-long-neak-ta");
            var duaBoBayNui = festivals.First(f => f.Slug == "dua-bo-bay-nui");

            // ===== Articles (10 bài viết mẫu) =====
            var articles = new List<Article>
            {
                new Article
                {
                    Title = "Giới thiệu lễ hội Chol Chnam Thmay của người Khmer",
                    Slug = "gioi-thieu-le-hoi-chol-chnam-thmay",
                    Summary = "Tìm hiểu ý nghĩa, thời gian, hoạt động chính trong lễ Chol Chnam Thmay – Tết cổ truyền của người Khmer.",
                    Content = "Chol Chnam Thmay là dịp quan trọng nhất trong năm đối với đồng bào Khmer Nam Bộ. Trong những ngày Tết, mọi người cùng nhau dọn dẹp nhà cửa, tắm Phật, đắp núi cát và tham gia nhiều trò chơi dân gian.",
                    CategoryId = categoryNews.CategoryId,
                    FestivalId = cholChnamThmay.FestivalId,
                    ThumbnailUrl = "/uploads/festivals/8946c6eb-e04a-4560-9072-068b8f4660e7.jpg",
                    PublishedDate = DateTime.UtcNow.AddDays(-10),
                    IsPublished = true,
                    AuthorId = editorAccount.AccountId,
                    CreatedAtUtc = DateTime.UtcNow.AddDays(-10)
                },
                new Article
                {
                    Title = "Ý nghĩa lễ Pithi Sene Dolta trong đời sống tinh thần người Khmer",
                    Slug = "y-nghia-le-pithi-sene-dolta",
                    Summary = "Lễ Sene Dolta là dịp để con cháu tưởng nhớ và tri ân ông bà tổ tiên.",
                    Content = "Trong những ngày diễn ra Sene Dolta, người Khmer tụ họp gia đình, dâng lễ vật lên chùa, mời sư sãi về tụng kinh và làm lễ cúng tại gia, thể hiện đậm nét đạo lý uống nước nhớ nguồn.",
                    CategoryId = categoryCulture.CategoryId,
                    FestivalId = seneDolta.FestivalId,
                    ThumbnailUrl = "/uploads/festivals/1a1f4b4f-2d9c-4a91-8d01-111111111111.jpg",
                    PublishedDate = DateTime.UtcNow.AddDays(-9),
                    IsPublished = true,
                    AuthorId = editorAccount.AccountId,
                    CreatedAtUtc = DateTime.UtcNow.AddDays(-9)
                },
                new Article
                {
                    Title = "Kinh nghiệm tham gia lễ hội Ok Om Bok và đua ghe Ngo",
                    Slug = "kinh-nghiem-tham-gia-ok-om-bok",
                    Summary = "Một vài gợi ý chuẩn bị, di chuyển, ăn uống khi bạn muốn tham gia lễ hội Ok Om Bok.",
                    Content = "Để có trải nghiệm trọn vẹn tại lễ hội Ok Om Bok, bạn nên chuẩn bị trang phục gọn nhẹ, tìm hiểu trước lịch trình đua ghe Ngo và đặt phòng lưu trú sớm trong mùa cao điểm.",
                    CategoryId = categoryTravel.CategoryId,
                    FestivalId = okOmBok.FestivalId,
                    ThumbnailUrl = "/uploads/festivals/2b2f4b4f-2d9c-4a91-8d01-222222222222.jpg",
                    PublishedDate = DateTime.UtcNow.AddDays(-8),
                    IsPublished = true,
                    AuthorId = editorAccount.AccountId,
                    CreatedAtUtc = DateTime.UtcNow.AddDays(-8)
                },
                new Article
                {
                    Title = "Lễ Chôl Vôsa – nghi lễ nhập hạ của chư tăng Khmer",
                    Slug = "le-chol-vosa-nghi-le-nhap-ha",
                    Summary = "Tìm hiểu ý nghĩa và các hoạt động chính trong lễ Chôl Vôsa.",
                    Content = "Lễ Chôl Vôsa đánh dấu thời gian chư tăng an cư kiết hạ. Người dân mang lễ vật tới chùa, dâng cúng để hộ trì Tam bảo và cầu bình an cho gia đình.",
                    CategoryId = categoryCulture.CategoryId,
                    FestivalId = cholVosa.FestivalId,
                    ThumbnailUrl = "/uploads/festivals/3c3f4b4f-2d9c-4a91-8d01-333333333333.jpg",
                    PublishedDate = DateTime.UtcNow.AddDays(-7),
                    IsPublished = true,
                    AuthorId = editorAccount.AccountId,
                    CreatedAtUtc = DateTime.UtcNow.AddDays(-7)
                },
                new Article
                {
                    Title = "Bon Phnôm Pôn – Lễ Ngàn Núi tạ lỗi với muôn loài",
                    Slug = "bon-phnom-pon-le-ngan-nui",
                    Summary = "Giải thích ý nghĩa nhân văn của lễ Bon Phnôm Pôn trong đời sống tâm linh người Khmer.",
                    Content = "Người Khmer quan niệm mình có lỗi với các sinh vật vì đã giết để làm thức ăn, nên tổ chức Bon Phnôm Pôn để đắp núi cát, cúng bái và xin các loài vật tha thứ.",
                    CategoryId = categoryCulture.CategoryId,
                    FestivalId = bonPhnomPon.FestivalId,
                    ThumbnailUrl = "/uploads/festivals/4d4f4b4f-2d9c-4a91-8d01-444444444444.jpg",
                    PublishedDate = DateTime.UtcNow.AddDays(-6),
                    IsPublished = true,
                    AuthorId = editorAccount.AccountId,
                    CreatedAtUtc = DateTime.UtcNow.AddDays(-6)
                },
                new Article
                {
                    Title = "Hội làng Bon Kâm San Srok – lễ cầu an của phum sóc",
                    Slug = "hoi-lang-bon-kam-san-srok",
                    Summary = "Lễ cầu an sau Tết Chol Chnam Thmay với nhiều hoạt động văn nghệ, trò chơi dân gian.",
                    Content = "Trong Bon Kâm San Srok, người dân thỉnh sư sãi đọc kinh cầu an, tế các vị thần bảo vệ đất đai và tổ chức múa hát, trò chơi dân gian suốt nhiều ngày đêm.",
                    CategoryId = categoryNews.CategoryId,
                    FestivalId = bonKamSanSrok.FestivalId,
                    ThumbnailUrl = "/uploads/festivals/5e5f4b4f-2d9c-4a91-8d01-555555555555.jpg",
                    PublishedDate = DateTime.UtcNow.AddDays(-5),
                    IsPublished = true,
                    AuthorId = editorAccount.AccountId,
                    CreatedAtUtc = DateTime.UtcNow.AddDays(-5)
                },
                new Article
                {
                    Title = "Lễ dâng Y Kathina – mùa tri ân Tam Bảo",
                    Slug = "le-dang-y-kathina-mua-tri-an",
                    Summary = "Giới thiệu nghi lễ dâng y cà sa và vật phẩm cho chư tăng trong dịp Kathina.",
                    Content = "Lễ Dâng Y Kathina là một trong những ngày lễ lớn của Phật giáo Nam tông. Phật tử Khmer rước Kathina quanh phum sóc, dâng y và các vật dụng sinh hoạt tới chư tăng trong không khí trang nghiêm nhưng vẫn rất rộn ràng.",
                    CategoryId = categoryCulture.CategoryId,
                    FestivalId = dangYKathina.FestivalId,
                    ThumbnailUrl = "/uploads/festivals/6f6f4b4f-2d9c-4a91-8d01-666666666666.jpg",
                    PublishedDate = DateTime.UtcNow.AddDays(-4),
                    IsPublished = true,
                    AuthorId = editorAccount.AccountId,
                    CreatedAtUtc = DateTime.UtcNow.AddDays(-4)
                },
                new Article
                {
                    Title = "Miakha Bôchia – rằm tháng Giêng của người Khmer",
                    Slug = "miakha-bochia-ram-thang-gieng",
                    Summary = "Lễ Miakha Bôchia tưởng nhớ các sự kiện trọng đại trong lịch sử Phật giáo.",
                    Content = "Vào ngày rằm tháng Giêng, chư tăng và Phật tử tụ hội về chùa tụng kinh, thắp nến và dâng lễ để tưởng nhớ Đức Phật và bày tỏ lòng tôn kính đối với Tam Bảo.",
                    CategoryId = categoryCulture.CategoryId,
                    FestivalId = miakhaBochia.FestivalId,
                    ThumbnailUrl = "/uploads/festivals/7a7f4b4f-2d9c-4a91-8d01-777777777777.jpg",
                    PublishedDate = DateTime.UtcNow.AddDays(-3),
                    IsPublished = true,
                    AuthorId = editorAccount.AccountId,
                    CreatedAtUtc = DateTime.UtcNow.AddDays(-3)
                },
                new Article
                {
                    Title = "Lễ hội Đom Lơng Néak Tà – di sản văn hóa phi vật thể quốc gia",
                    Slug = "le-hoi-dom-long-neak-ta-di-san",
                    Summary = "Khám phá lễ hội cúng Néak Tà của người Khmer Trà Vinh, vừa được công nhận di sản quốc gia.",
                    Content = "Lễ hội Đom Lơng Néak Tà được tổ chức tại các miếu Néak Tà với nghi thức cúng bái, dâng lễ và nhiều hoạt động văn hóa, thể thao dân gian, góp phần gắn kết cộng đồng và phát triển du lịch văn hóa – tâm linh.",
                    CategoryId = categoryNews.CategoryId,
                    FestivalId = domLongNeakTa.FestivalId,
                    ThumbnailUrl = "/uploads/festivals/8b8f4b4f-2d9c-4a91-8d01-888888888888.jpg",
                    PublishedDate = DateTime.UtcNow.AddDays(-2),
                    IsPublished = true,
                    AuthorId = editorAccount.AccountId,
                    CreatedAtUtc = DateTime.UtcNow.AddDays(-2)
                },
                new Article
                {
                    Title = "Đua bò Bảy Núi – sắc màu lễ hội vùng biên giới An Giang",
                    Slug = "dua-bo-bay-nui-sac-mau-le-hoi",
                    Summary = "Giới thiệu không khí sôi động của lễ hội đua bò Bảy Núi gắn với lễ Dolta.",
                    Content = "Mỗi mùa lễ Dolta, hàng chục đôi bò cùng nông dân vùng Bảy Núi tề tựu tranh tài trên đường đua bùn. Tiếng hò reo cổ vũ và sắc màu trang trí rực rỡ tạo nên bản sắc rất riêng của lễ hội đua bò An Giang.",
                    CategoryId = categoryTravel.CategoryId,
                    FestivalId = duaBoBayNui.FestivalId,
                    ThumbnailUrl = "/uploads/festivals/9c9f4b4f-2d9c-4a91-8d01-999999999999.jpg",
                    PublishedDate = DateTime.UtcNow.AddDays(-1),
                    IsPublished = true,
                    AuthorId = editorAccount.AccountId,
                    CreatedAtUtc = DateTime.UtcNow.AddDays(-1)
                }
            };

            context.Articles.AddRange(articles);
            context.SaveChanges();

            // ===== Contacts (một vài liên hệ mẫu) =====
            if (!context.Contacts.Any())
            {
                var contacts = new List<Contact>
                {
                    new Contact
                    {
                        FullName = "Nguyễn Văn A",
                        Email = "visitor1@example.com",
                        Subject = "Hỏi về lịch tổ chức lễ Chol Chnam Thmay",
                        Message = "Cho mình hỏi năm nay lễ Chol Chnam Thmay ở Sóc Trăng tổ chức chính vào ngày nào?",
                        Status = 0,
                        CreatedAtUtc = DateTime.UtcNow.AddDays(-2)
                    },
                    new Contact
                    {
                        FullName = "Trần Thị B",
                        Email = "visitor2@example.com",
                        Subject = "Tư vấn địa điểm tham gia đua ghe Ngo",
                        Message = "Mình muốn xem đua ghe Ngo thì nên đến tỉnh nào và di chuyển ra sao?",
                        Status = 0,
                        CreatedAtUtc = DateTime.UtcNow.AddDays(-1)
                    }
                };
                context.Contacts.AddRange(contacts);
                context.SaveChanges();
            }
        }

        private static (byte[] hash, byte[] salt) HashPassword(string password)
        {
            using (var hmac = new HMACSHA512())
            {
                var salt = hmac.Key;
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return (hash, salt);
            }
        }
    }
}
