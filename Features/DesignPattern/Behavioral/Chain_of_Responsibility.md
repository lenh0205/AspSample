> the common point of other **behavioral design patterns** is that they rely on **converting specific behaviors** into **`standalone objects called "handlers"`** 

# Chain of Responsibility / CoR / Chain of Command
* -> lets us **`pass requests along a chain of handlers`** - upon receiving a request, each handler decides either to process the request or to pass it to the next handler in the chain

## Summary
* -> tức là thay vì 1 class với nhiều behavior ta nên tách những behavior này ra; bỏ vào class riêng biệt (gọi là **`Handler`**) với 1 method duy nhất để handling requests (nhưng mà nhiều khi cũng có method khác để setting the next handler on the chain) 
* -> it's crucial that **`all handler classes implement the same interface`** - each concrete handler should only care about the following one having the "execute" method
* -> rồi sau đó ta **link these handlers into a chain** - by **`passing a next handler to the constructor (or setter) of handler`** and has **`a field for storing a reference to the next handler`**
* -> **a handler** is **`usually self-contained and immutable`** can **`decide whether to process it, pass the request further down the chain, or effectively stop any further processing`**

## Extend 
* -> ta có thể thêm Base Handler - an optional class where we can put the boilerplate code that's common to all handler classes - for example, implement the default handling behavior: it can pass execution to the next handler after checking for its existence
* -> there’s a slightly different approach (and it’s a bit more canonical) in which, upon receiving a request, a handler decides whether it can process it. If it can, it doesn’t pass the request any further (_very common when dealing with events in stacks of elements within a graphical user interface_)

## Example

```java
// -> "ComponentWithContextualHelp" là Handler interface; "Component" là base class chứa default implementation cho method chính của handler và "container" field để reference tới next handler
// -> "Container" là handler - với add() method để thêm mới children (1 handler) đồng thời thời xét nó làm container của children (xét nó làm next handler của children)  

// The handler interface declares a method for executing a
// request.
interface ComponentWithContextualHelp is
    method showHelp()


// The base class for simple components.
abstract class Component implements ComponentWithContextualHelp is
    field tooltipText: string

    // The component's container acts as the next link in the
    // chain of handlers.
    protected field container: Container

    // The component shows a tooltip if there's help text
    // assigned to it. Otherwise it forwards the call to the
    // container, if it exists.
    method showHelp() is
        if (tooltipText != null)
            // Show tooltip.
        else
            container.showHelp()


// Containers can contain both simple components and other
// containers as children. The chain relationships are
// established here. The class inherits showHelp behavior from
// its parent.
abstract class Container extends Component is
    protected field children: array of Component

    method add(child) is
        children.add(child)
        child.container = this


// Primitive components may be fine with default help
// implementation...
class Button extends Component is
    // ...

// But complex components may override the default
// implementation. If the help text can't be provided in a new
// way, the component can always call the base implementation
// (see Component class).
class Panel extends Container is
    field modalHelpText: string

    method showHelp() is
        if (modalHelpText != null)
            // Show a modal window with the help text.
        else
            super.showHelp()

// ...same as above...
class Dialog extends Container is
    field wikiPageURL: string

    method showHelp() is
        if (wikiPageURL != null)
            // Open the wiki help page.
        else
            super.showHelp()


// Client code.
class Application is
    // Every application configures the chain differently.
    method createUI() is
        dialog = new Dialog("Budget Reports")
        dialog.wikiPageURL = "http://..."
        panel = new Panel(0, 0, 400, 800)
        panel.modalHelpText = "This panel does..."
        ok = new Button(250, 760, 50, 20, "OK")
        ok.tooltipText = "This is an OK button that..."
        cancel = new Button(320, 760, 50, 20, "Cancel")
        // ...
        panel.add(ok)
        panel.add(cancel)
        dialog.add(panel)

    // Imagine what happens here.
    method onF1KeyPress() is
        component = this.getComponentAtMouseCoords()
        component.showHelp()
```