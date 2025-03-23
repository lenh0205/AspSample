https://dev.to/kalkwst/adapter-pattern-in-c-305a
https://dotnetcorecentral.com/blog/adapter-pattern/
https://www.ezzylearning.net/tutorial/adapter-design-pattern-in-asp-net-core
https://stackoverflow.com/questions/60686193/adapter-pattern-for-viewmodels-in-asp-net-core-mvc

=================================================================
# Adapter / Wrapper
* -> nó là 1 class hoạt động như 1 wrapper của imcompatible input; behavior thay vì sử dụng input thì sẽ sử dụng Adapter của input đó

## Summary
* -> Use the Adapter class when you want to use some existing class, but its interface isn’t compatible with the rest of your code
* => nhưng ta cần lưu ý The overall complexity of the code increases because you need to introduce a set of new interfaces and classes. Sometimes it’s simpler just to change the service class so that it matches the rest of your code.
* => nhưng sẽ có những trường hợp ta không thể can thiệp vào code của service class (often 3rd-party, legacy or with lots of existing dependencies)