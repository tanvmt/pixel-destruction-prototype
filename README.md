Pixel Destruction
====================

Một tựa game 2D Hyper-casual thỏa mãn thị giác (Satisfying Game), nơi người chơi điều khiển các loại vũ khí (như cưa máy) để phá hủy các khối vật thể khổng lồ được cấu tạo từ hàng ngàn hạt Pixel vật lý.

Dự án tập trung mạnh vào việc tối ưu hóa hiệu năng (Performance Optimization) cho thiết bị di động khi xử lý số lượng lớn physics colliders, đồng thời áp dụng kiến trúc Data-Driven để dễ dàng thiết kế và mở rộng màn chơi.

### 1. Hướng dẫn mở project và chạy
----------------------------------

### Yêu cầu hệ thống:

*   **Unity Editor:** Khuyến nghị phiên bản 2022.3 LTS trở lên.
    
*   **Môi trường Build:** Đã cài đặt Android Build Support (nếu muốn build ra file APK).
    

### Các bước cài đặt:

1.  Bashgit clone
    
2.  **Mở Project:** Mở Unity Hub, chọn **Add project from disk** và trỏ tới thư mục vừa clone.
    
3.  **Cài đặt Package (Quan trọng):**
    
    *   Dự án sử dụng **DOTween (HOTween v2)** cho hệ thống UI Animation.
        
    *   Nếu mở project lên bị báo lỗi đỏ ở các file UI, hãy vào Window -> Asset Store (hoặc Package Manager), tìm và import bản DOTween miễn phí.
        
4.  **Chạy Game:**
    
    *   Mở scene chính tại đường dẫn: Assets/\_Project/Scenes/Gameplay.unity (thay đổi đường dẫn nếu cần).
        
    *   Nhấn nút **Play** trên Unity Editor để trải nghiệm.
        

### 2. Kiến trúc Code (Core Systems)
------------------------------------

Dự án được xây dựng theo hướng module hóa, tách biệt rõ ràng giữa Logic, Data và Visual, bao gồm các hệ thống chính sau:

*   **Hệ thống Object Pooling (PoolManager):** Thay vì Instantiate/Destroy liên tục gây tràn bộ nhớ (Garbage Collection Spikes), toàn bộ hạt gạch (PixelNode), khối vật thể (PixelObject), và hiệu ứng VFX (SawHitVFX, TapFlashVFX) đều được quản lý bằng Object Pool.
    
*   **Kiến trúc Data-Driven (LevelData & LevelManager):** Mỗi Level không phải là một Scene riêng biệt mà là một ScriptableObject chứa dữ liệu (Tọa độ spawn, danh sách Texture, Máu của pixel, Độ khó). LevelManager sẽ đọc dữ liệu này để tự động "lắp ráp" màn chơi ngay trong runtime.
    
*   **Trình tạo lưới Vật lý (PixelSpawner):** Hệ thống tự động đọc một file ảnh 2D (Texture2D), lọc các pixel trong suốt, và khởi tạo một mạng lưới các PixelNode mang theo màu sắc nguyên bản, Rigidbody2D và BoxCollider2D.
    
*   **Hệ thống Phá hủy & Va chạm (Destruction Logic):** Khi vũ khí (Circular Saw) cắt qua vật thể, nó sẽ dò tìm các PixelNode lân cận, trừ máu và tách chúng ra khỏi khối mẹ. Các hạt rơi xuống tuân thủ các quy tắc vật lý được tối ưu (Triệt tiêu độ nảy, tự động tính khối lượng).
    
*   **Hệ thống UI & Visual Effects:** Sử dụng DOTween cho các animation nảy/bung cửa sổ mượt mà. Hệ thống Trail Renderer và Particle System được thiết lập qua Layer Sorting để luôn hiển thị chính xác độ sâu.
    

### 3. Hướng dẫn sử dụng Level Editor Tool
-----------------------------------------

Để đẩy nhanh quá trình thiết kế màn chơi mà không cần can thiệp vào code, dự án tích hợp sẵn một Custom Editor Window chuyên dụng.

### Cách sử dụng:

1.  Trên thanh menu của Unity, chọn Tools -> Pixel Destruction -> Level Editor. Cửa sổ Tool sẽ hiện ra.
    
2.  **Tạo Level mới:** Nhập tên level vào ô _Create New Level_ và bấm **Create**. Một file LevelData mới sẽ được tự động tạo trong thư mục Assets.
    
3.  **Thiết lập Gameplay:** Chỉnh sửa số lượng khối tối đa, thời gian spawn, lượng máu (Node Health) trực tiếp trên Tool.
    
4.  **Thiết kế Môi trường:**
    
    *   Kéo thả các Prefab Tường/Chướng ngại vật từ Project vào các vùng chứa DynamicContainer hoặc StaticContainer trên Scene.
        
    *   Dùng công cụ Move/Rotate/Scale của Unity để sắp xếp bố cục (Ví dụ: tạo một cái phễu tử thần).
        
5.  **Lưu Dữ liệu:** Nhấn nút màu xanh lá **2\. Scan Scene & Save to Data**. Tool sẽ tự động quét toàn bộ tọa độ trên Scene và lưu chết vào file LevelData. Khi Play game, màn chơi sẽ được load chính xác 100%.
    

> **Hình ảnh minh họa:**
> _(Cửa sổ Level Editor Window)_
> > <img width="349" height="553" alt="image" src="https://github.com/user-attachments/assets/ee96c558-bd79-4ef8-a2f4-34bb91ce3c72" />

### 4. Những điều sẽ cải thiện (Nếu có thêm thời gian)
-----------------------------------------------------

Dù dự án đã có bộ khung core-loop hoàn chỉnh, nhưng để đạt được chất lượng thương mại hóa (Production-ready), đây là những điểm mình sẽ ưu tiên nâng cấp:

*   **Hệ thống Audio Manager (Pooling):** Triển khai Object Pooling cho các AudioSource để xử lý hàng chục âm thanh gạch vỡ và tiếng rít của cưa máy cùng lúc mà không bị cắt tiếng hay rè loa.
    
*   **Tối ưu:** Tiếp tục tối ưu hiệu năng bởi vì vẫn còn khá giật lag khi nhiều pixel được sinh ra cùng lúc.
    
*   **Mở rộng kho Vũ khí & Cơ chế:** Thêm các loại vũ khí tương tác vật lý khác như Búa, Súng phun lửa hoặc Laser.
