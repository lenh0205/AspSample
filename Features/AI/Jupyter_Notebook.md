=============================================================
# Jupyter Notebook
* nói chung là nó cho phép ta viết các block code và block markdown xen kẽ nhau; ta cứ Shift+Enter để chạy từng dòng như 1 REPL, gặp block code thì nó sẽ show kết quả như Console vậy 
* -> nền tảng tính toán khoa học mã nguồn mở ghép từ ba ngôn ngữ lập trình Julia, Python và R
* -> cốt lõi là **Markdown** - đưa dữ liệu, code, hình ảnh, công thức, video,.. vào trong cùng một file; ta có thể vừa trình chiếu vừa chạy code để tương tác trên đó
* => bạn có thể sử dụng để tạo, chia sẻ các tài liệu có chứa code trực tiếp, phương trình, trực quan hóa dữ liệu và văn bản tường thuật
* => được coi là môi trường điện toán tương tác đa ngôn ngữ, hỗ trợ hơn 40 ngôn ngữ lập trình cho người dùng

## Exploratory Data Analysis
* -> cho phép người dùng xem kết quả của code in-line (mã inline) mà không cần phụ thuộc vào các phần khác của code
* -> mọi ô của code có thể được kiểm tra bất cứ lúc nào (khác biệt so với các ID như Pycharm, VSCode)

## Độc lập ngôn ngữ

## Data Visualisation

## Tương tác trực tiếp với code

##  Các mẫu code tài liệu

=============================================================
# cài đặt Jupyter Notebook

```bash
$ Python --Version
$ pip3 --version
$ pip3 install jupyter
```

# sử dụng Jupyter Notebook

* -> điều hướng đến thư mục mà ta muốn lưu sổ ghi chép của mình + chạy Jupyter
```bash
# khởi tạo một máy chủ cục bộ tại http://localhost:8888/tree (try cập Browser để xem)
$ jupyter notebook
```

* -> tạo 1 Notebook cơ bản: click "New" + chọn loại Notebook tài liệu (Python, Text file, Folder...)
* -> hoặc nếu muốn sử dụng 1 Notebook jupyter khác trên hệ thống thì nhấp "Upload" + điều hướng đến tệp đó
* => Jupyter Notebook của ta sẽ được mở trong tab mới - 1 file **jSON** có extension là **`.jpynb`** (nhưng ta vẫn có thể xuất Notebook jupyter ở dạng khác như HTML)
* => Notebook có biểu tượng màu xanh lá cây là đang chạy; còn màu xám là không chạy
* => quay lại dashboard, ta sẽ thấy file mới Untitled.ipynb 

* -> Làm việc với Notebook
* một notebook bao gồm nhiều cell (ô) - khi tạo mới một notebook, ta luôn được tạo sẵn một cell rỗng đầu tiên; mở menu để insert thêm cell bên dưới
* mặc định cell mới tạo sẽ là code cell
* **`Cell`** - có nội dung là code hoặc markdown được hiển thị trong Jupyter Notebook, đây là vùng chúng ta sẽ làm việc chính
* **`Kernel`** - là một "công cụ tính toán" thực thi code có trong Jupyter Notebook
* _nhấp "Run" hoặc "Shift + Enter" để chạy Cell_

## Cell
* _Code cell - thực thi bởi kernel hiển thị đầu ra bên dưởi code cell; Markdown cell - hiện thị đầu ra ngay tại chỗ nó chạy cell_
* _bên trái code cell sẽ có label ln[]; nếu chạy lần đầu trên kernel nó sẽ chuyển thành ln[1]_

## Kernel
* _kernel có 1 số tùy chọn:_
* -> **Restart** - Khởi động lại kernel nó sẽ xóa tất cả các biến đã được xác định.
* -> **Restart và Clear Output** - Sẽ xóa đầu ra được hiển thị bên dưới các code cell của bạn.
* -> **Restart & Run All** - Sẽ chạy lại tất cả các cell của bạn theo thứ tự từ đầu đến cuối.

## Một số cú pháp trong Markdown
* -> tạo Blockquotes: sử dụng ">" hoặc "<blockquote></blockquote>"
* -> tạo bẳng - Ex: |Name|Address|Salary| |-----|-------|------| |Hanna|Brisbane|4000| |Adam|Sydney|5000|

## Help
* -> ta sẽ thấy liên kết đến các tài liệu khác nhau dành cho các mô-đun như Numpy, SciPy và Matplotlib
