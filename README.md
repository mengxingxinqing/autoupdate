# autoupdate
winform程序的自动更新程序

## 使用方法见博客园文章
[博客园文章地址](http://www.cnblogs.com/mengxingxinqing/p/6517299.html)

## 装到windows系统盘，无法自动更新
添加app.manifest  文件进行配置设定即可
```
<requestedPrivileges xmlns="urn:schemas-microsoft-com:asm.v3">
 <requestedExecutionLevel level="requireAdministrator" uiAccess="false" />  
</requestedPrivileges>
```