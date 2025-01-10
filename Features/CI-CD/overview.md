
* -> tính năng cơ bản sẽ là sau khi push code sẽ tự động build
* -> ta có thể sử dụng GitLab để CI/CD - chạy tự động các script; và Bash để viết các script

* -> có thể chọn option là sử dụng hệ thống Remote server của Gitlab để build luôn nhưng sẽ bị hạn chế
* -> thường thì ta sẽ có 1 máy tính riêng làm host có đầy đủ môi trường để build, trên máy cài Gitlab Client
* -> Gitlab server sẽ móc đến client này để nó build ra product và sử dụng SSH để đẩy product qua Server chính
