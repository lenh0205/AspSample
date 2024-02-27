# namespace sinh ra cho mục đích gì ?
* 1 chương trình C# được tổ chức bởi những namespace
-> hay namespace được dùng như hệ thống tổ chức cho 1 chương trình
-> đại diện cho những thành phần của 1 program khi exposed cho những chương trình khác

# namespace cần làm gì để đạt được mục đích đó
* namespace dùng để tạo scope để chứa những objects liên quan tới nhau
=> nó giúp ta tổ chức các thành phần của code và viết những type global 1 cách unique

* có 1 namespace mặc định được compiler add vô để chứa những khai báo chưa có namespace
-> nó thường được gọi là "global namespace"
-> các namespace khác có khả năng sử dụng các thành phần trong global namespace 

# có 2 cách viết namespace:
* -> normal (sử dụng "{}"):
namespace MyNameSpace
{
    // other_namespace, class, interface, struct, enum, delegate
}

* -> File scoped namespace (tức 1 file .cs ứng với 1 namespace):
namespace MyNameSpace;
class ...
interface ...
struct...
enum...
delegate...
// (không thể chứa other_namespace)

# Compilation Unit


