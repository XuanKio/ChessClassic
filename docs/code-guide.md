# Đọc code PixelShooter

## Ý tưởng chính

Mỗi component làm một việc rõ ràng. Một GameObject ghép nhiều component lại thành nhân vật hoặc vũ khí. Đây là cách tổ chức OOP hiện tại: dữ liệu được đóng gói, các đối tượng phối hợp qua phương thức và event.

**S:** thay cách hiện crosshair thì sửa CrosshairView; thay cách tạo đạn thì sửa ProjectileSpawner.

**O:** đổi thông số bằng WeaponData hoặc đổi hình/cấu hình đạn bằng prefab, không cần sửa luồng bắn trong WeaponController. Hiện chưa cần nhiều tầng kế thừa hay interface.

## Thư mục tìm nhanh

```text
Assets/
  _Game/
    Art/          Sprites, Environments, References
    Audio/        Âm thanh AssetGame
    Fonts/        Font gốc AssetGame và SDF cho HUD
    Materials/    Material Characters, Environments, VFX
    Shaders/      Mã shader
    Animations/   Clip và Animator controller
    Scenes/       Bootstrap, SampleScene
    Prefabs/      Environments, Projectiles, VFX
    Scripts/      Code chia theo chức năng
    ScriptableObjects/  Cấu hình Characters, Weapons
    Settings/     Input và Rendering
  Screenshots/    Giữ thư mục; dọn ảnh tạm khi dùng xong
  TextMesh Pro/   Tài nguyên đi kèm TMP
```

Ảnh tham khảo Sample A/B nằm trong Art/References. Material nằm trong Materials; shader nằm trong Shaders. Khi đổi vị trí asset, di chuyển bằng Unity để giữ GUID và các tham chiếu.

## 1. Đọc theo luồng người chơi

```text
PlayerControls.inputactions
        ↓
PlayerInputHandler — đọc thao tác
        ↓
PlayerController — chuyển yêu cầu đến đúng component
        ├── CharacterMovement — di chuyển
        ├── CharacterAim — hướng súng và flip
        └── WeaponController — bắn, số đạn, nạp
                    ↓
            ProjectileSpawner — tạo prefab đạn
                    ↓
            ProjectileMovement — đạn bay và chạm vật cản
```

PlayerInputHandler đọc trước PlayerController nhờ execution order -50. Mỗi Player dùng một bản Input Actions riêng trong runtime; không thay đổi asset dùng chung.

| Thao tác | Action |
|---|---|
| WASD | Move |
| Di chuột | Aim |
| Chuột trái hoặc phải | Fire |
| R | Reload |

## 2. Muốn chỉnh gì thì mở đâu?

| Muốn chỉnh | Nơi chỉnh |
|---|---|
| Tốc độ, tăng tốc, giảm tốc | CharacterData đang gán trên CharacterMovement |
| Nhịp bắn, tốc độ đạn, thời gian tồn tại, số viên, thời gian nạp | Assets/_Game/ScriptableObjects/Weapons/StarterWeaponData.asset |
| Sprite, màu, kích thước, sorting, bán kính/mask va chạm đạn | Assets/_Game/Prefabs/Projectiles/PlayerBullet.prefab |
| Vật cản chặn ngay đầu nòng | Blocking Layers trên ProjectileSpawner |
| Đổi phím | Assets/_Game/Settings/Input/PlayerControls.inputactions |

WeaponData chỉ lưu cấu hình. Số đạn đang có và thời gian nạp nằm trong WeaponController của từng khẩu súng.

## 3. Vòng đời bắn và nạp

WeaponController kiểm tra thời gian giữa hai phát → nhờ ProjectileSpawner tạo đạn → chỉ trừ một viên nếu tạo thành công. Khi đầu nòng bị vật cản chặn, không trừ đạn.

Hết băng tự nạp. R nạp sớm nếu băng chưa đầy. Trong lúc nạp không bắn; nạp xong đầy băng, giữ chuột sẽ bắn tiếp. Hiện đạn dự trữ là vô hạn.

HUD nghe OnAmmoChanged và OnReloadStateChanged để hiện số đạn hoặc `...`. Gameplay không gọi trực tiếp TMP hay Canvas.

## 4. Máu, chết và tắt component

CharacterHealth sở hữu máu và trạng thái chết. Máu tự khởi tạo đúng một lần khi đọc lần đầu hoặc Awake, nên không cần ép thứ tự chạy so với UI hay Controller.

Khi chết: PlayerController dừng và khóa di chuyển; hủy bắn/nạp. CharacterPresentation chỉ đổi animation và hit flash qua shader. Rigidbody2D vẫn được mô phỏng.

Khi chỉ tắt PlayerController: ClearMoveInput dừng ngay nhưng không khóa vĩnh viễn, CancelActions ngừng yêu cầu bắn/nạp. Bật lại Controller khi còn sống có thể nhận input tiếp.

CrosshairView tự cập nhật vị trí tâm và quản lý ẩn/hiện con trỏ. Aim không biết Canvas. HealthBarView đọc lại máu khi bật lại để không hiện dữ liệu cũ.

## 5. Quy tắc khi thêm tính năng

Trước hết xác định component sở hữu trạng thái đó. Cho component khác gọi phương thức để yêu cầu, hoặc nghe event để cập nhật hình ảnh. Chỉ thêm class khi có trách nhiệm đủ rõ để đặt tên.

Hiện chưa có respawn, pooling hay server authority. Đạn mới chặn môi trường, chưa gọi IDamageable lên nhân vật. Khi làm multiplayer, server phải quyết định sát thương, số đạn và việc một phát bắn có hợp lệ hay không.

## Asset còn dùng và file tạm

PrototypeArena là prefab đang được SampleScene dùng cho gốc map, gồm hàng rào va chạm và các cụm cỏ. Không xóa prefab hoặc atlas/tile mà nó còn tham chiếu chỉ vì tên có chữ Prototype.

Giữ thư mục Assets/Screenshots để chụp lần sau. Chỉ xóa ảnh chụp đã dùng xong cùng metadata của từng ảnh bằng Unity; không xóa thư mục hoặc metadata của thư mục. Art gốc trong AssetGame là thư viện để tiếp tục dựng nội dung; việc chưa xuất hiện trong scene không có nghĩa là asset đó cần xóa.
