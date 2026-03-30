# Eco Puzzle
**Eco Puzzle** là một trò chơi thuộc thể loại **Puzzle Strategy** kết hợp mô phỏng môi trường trên nền tảng Mobile. Người chơi sẽ nhập vai vào người kiến tạo, sử dụng các chiến thuật biến đổi ô đất khô cằn thành những hệ sinh thái xanh tươi, tràn đầy sự sống và các loài động vật.

### Liên kết dự án
* Chơi thử: [Download APK](https://tins24.itch.io/eco-flow)
* Video Demo: [Youtube](https://www.youtube.com/shorts/BD2eSSKDNOU)

## Gameplay Overview
#### Core Loop
Chơi màn → Biến đổi môi trường/Đặt động vật → Hoàn thành nhiệm vụ → Nhận thưởng & Mở khóa → Tái tạo khu vực mới.

#### Cơ chế điều khiển & Quy tắc
Trò chơi sử dụng thao tác Long Press (Nhấn giữ) tối giản nhưng mang tính chiến thuật cao:
* Tạo nước (Hồ): Nhấn giữ ô đất khô cằn.
* Phát triển thảm thực vật: Nước xuất hiện sẽ tự động xanh hóa các ô xung quanh thành cỏ.
* Xây dựng Rừng: Kết hợp các cụm cây xanh (3, 4 hoặc ≥5 cây) để tạo ra các cấp độ Rừng khác nhau.
* Hệ sinh thái đặc biệt: Kết hợp Đá + Nước để tạo cỏ vàng, hoặc nhiều Hồ nước để tạo cánh đồng hoa.

#### Hệ thống Động vật
Mỗi loài động vật yêu cầu một môi trường sống riêng biệt:
* Lợn rừng/Sói: Rừng.
* Báo/Voi: Đồng cỏ vàng.
* Ong/Bướm: Cánh đồng hoa.

## Cấu trúc thư mục
```text
Assets/
├── Scripts/
│   ├── Core/             # Grid Manager, Turn System
│   ├── Grid/             
│       ├── Tile/         # BaseTile & Derived Classes (Forest, Water, etc.)
│   ├── Animal/           # Animal Data & Logic
│   ├── Booster/          # Item logic
│   ├── System/           # Object Pool, SceneTransition, etc.
│   ├── Level/            # Level manager
│   ├── Home/             # Home UI and Logic
│   ├── Tutorial/         # Tutorial manager
│   ├── UI/               # UI manager
│   └── FireBase/         # Analytics
├── Prefabs/              # Tiles
├── Audio/                # Mixer
├── FancyScrollView/      # Source Scroll View
├── Resources/Data/Level/ # Custom Level Creator Tool
└── Editor/               # Create Tool
```

## Art Style & UI/UX
* Phong cách: Low-poly/Flat design, màu sắc tươi sáng, gradient nhẹ nhàng mang lại cảm giác thư giãn.
* Hiệu ứng: Sử dụng Tweening cho các chuyển động "Pop" của động vật và animation mềm mại khi biến đổi môi trường.
* UI: Thiết kế tối ưu cho trải nghiệm một tay trên Mobile.

## Hướng dẫn cài đặt
1. Clone repository (Nhánh develop nếu muốn cập nhật mới nhất):
```bash
git clone https://github.com/YourUsername/Eco-Puzzle.git
```
2. Mở dự án bằng Unity 6 hoặc mới hơn.
