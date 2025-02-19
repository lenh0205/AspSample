======================================================================================
# git rebase
* -> về cơ bản, có thể hiểu **git rebase** tương tự như **git merge** - đều dùng để kết hợp những thay đổi từ nhánh này qua nhánh kia, nhưng khác nhau về cơ chế hoạt động

## git merge
* -> là cách dễ và an toàn để hợp nhất commit từ 2 nhánh 
* -> nó sẽ phải tạo ra 1 commit để biểu hiện cho action merge này
* -> nó sẽ không phá hủy lịch sử commit của các nhánh nhưng vấn đề là nó làm lịch sử commit trông rất là rối
```bash
# ví dụ merge commit từ nhánh 'main' vào nhánh '#feature-a'
git checkout #feature-a
git merge main 
```
![git merge](https://topdev.vn/blog/wp-content/uploads/2023/06/Git-merge-rebase-4.webp)

## git rebase
* -> sẽ tạo ra lịch sử commit mới 
* -> đầu tiên tìm đến commit chung gần nhất (common ancestor commit) giữa hai nhánh, rồi đem hết commit của nhánh mà ta đang ở lên đầu của nhánh ta rebase lên (tính từ commit chung)
* => vậy nên lịch sử commit trông sẽ rất gọn gàng (1 đường thẳng)
```bash
git checkout #feature-a
git rebase main
```
![git merge](https://topdev.vn/blog/wp-content/uploads/2023/06/Git-merge-rebase-6.webp)

### Problem
* _VD: git rebase nhánh main lên nhánh #feature-a_
![problem](https://topdev.vn/blog/wp-content/uploads/2023/06/Git-merge-rebase-7.webp)

* -> vấn đề là giờ lịch sử nhánh main của ta với nhánh main của những người khác đang rất khác biệt
* -> giờ có 2 cách xử lý: 
* -> 1 là ta sẽ merge 2 nhánh này thành 1 để xử lý những conflict tồn tại nhưng nó sẽ có thể tạo một nùi commit giống nhau; 
* -> 2 là ta sẽ **git push --force** lên remote, khi team member kéo code về thì họ sẽ thấy là những commit xa lạ

* => vậy nên ta không nên git rebase nhánh riêng, không nên có nhiều người đang làm việc

### Advance
https://200lab.io/blog/git-rebase-vs-git-merge

======================================================================================
# Rebase with conflict
https://stackoverflow.com/questions/161813/how-do-i-resolve-merge-conflicts-in-a-git-repository
https://www.simplilearn.com/tutorials/git-tutorial/merge-conflicts-in-git
