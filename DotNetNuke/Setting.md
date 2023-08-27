------------------------------
_`Set up a new DNN 9 website (always do)`_ : _Delete content on default home page, site setting: turn off default feature, config registration, disable DNN copyright_

# Delete Modules
* Go to Edit mode -> Di chuyến Module -> setting icon -> Delete
* Xoá hết Module ở Home page

# Site Setting

## Site Info
* Điền `Title`, `Description`, `Keywords` will be utilize by DNN on any page that don't have their own config (for the SEO)
* `Site Time Zone`: choose time zone where the server is located
* `Copyright`: the copy right message is being displayed within the _skin or theme_ that controls look and feel of websites ; if remove all together what DNN will take the title + current year 
* `Logo and icon`: upload a new logo for site
* -> Save -> Refresh page

## Site Behavior
* -> More
* Uncheck `Check for Software Upgrades`, uncheck `participate in DNA improvement option`

# Security
* **Member Accounts** -> **Registration Settings** -> **User Registration**: 
* `None` - not allow users to register; not allow them to create new accounts as a host or an admin; can still create accounts for our users
* `Private` - allows people to register but they cannot login until approved by an administrator
* `Public` - anyone can register on the site and immediately login 
* `Verified` - allows to register on the site but have to go through a verification process (click on a link in an email that the site will send) to them for now in our development
* -> Save

# Delete the DNN commment on head tag of HTML source on every pages of DNN websites
* **SETTING** -> **SQL Console** -> Hiện tại không thể Write đc, cần Reload lại -> paste in sequel: `update hostsettings set settingvalue ='N' where settingname = 'Copyright'` (để update _hostsettings_ table; vì bình thương ta query thì nó sẽ ra 'Y', ta cần đổi thành 'N') -> Run Script
* **SETTING** -> **Server** -> `Clear Cache` -> `Restart Application` -> Reload page


----------------------------
_`Site Setting details of DNN 9`_

## Site Info:
* **Site Logo** : browse logo in DNN, Upload, Link
* **Favicon**: image, logo (`.ico` file) -> will show up at the Tab
* -> Save -> Reload page

## Site Behavior