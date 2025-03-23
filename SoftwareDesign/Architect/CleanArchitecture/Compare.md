========================================================================
# Compare

## Layers in 'Onion Architecture' and 'Hexagonal Architecture'
* -> Sơ đồ Kiến trúc **Hexagonal Architecture** chỉ hiển thị cho chúng ta hai layer: **`Bên trong ứng dụng`** và **`bên ngoài của ứng dụng`**

* -> mặt khác, **Onion Architecture** mang đến sự kết hợp các layer ứng dụng được xác định bởi **`DDD (Domain Driven Design)`**
* -> **Entities**
* -> **Value Objects**
* -> **Application Services** chứa các **`use-case logic`**
* -> **Domain Services** đóng gói **`domain logic không thuộc về các Entities hoặc Value Objects…`**

* -> _khi so sánh với `Onion Architecture`_, **Clean Architecture** sẽ duy trì "Application Services layer (Use Cases)" và "Entities layer" nhưng dường như nó quên mất **`Domain Services layer`**
* -> tuy nhiên thực tế trong Clean Architecture, **Entities** không chỉ là và Entity theo ý nghĩa của DDD mà bất cứ Domain object nào; tức là **`2 layer bên trong đã được sát nhập để đơn giản sơ đồ`**
* _"Một entity có thể là một đối tượng với các phương thức, hoặc nó có thể là một tập hợp các cấu trúc dữ liệu và các hàm"_
