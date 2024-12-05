# Befree

一款通过轮询各类订阅中节点的代理池工具

看了v2rayN、sstap等工具之后，发现很多类似的工具基本都是套了个壳，最后还是调用clash、xray等代理工具。

于是为了操作方便以及代理池的需求，有了这款工具。通过解析订阅中各个节点，重新生成clash配置文件，调用clash，进行节点轮询，达到代理池的效果。

本来打算调用clash这部分也重新写一下，后来发现很多协议、很多加解密类型，太繁琐，造轮子有点麻烦，于是最后决定还是直接调用clash。

release中的文件包含了一个clash，如果怕存在后门，可以直接下载源码修改befree.csproj以及clashrunner.cs中的clash文件路径至自己的clash。

## 更新记录

v0.3版本
* 修复了lssues中提到的0.2版本的url编码问题
* 添加了指定yaml文件的参数，可通过-y自定义yaml配置文件

v0.2版本
* 解决了利用时经常会轮询到不可用的节点，导致代理效果不佳，部分请求出错的问题。
* 通过添加-t参数指定测速的url链接。程序运行后会进行测速，可以稍等一会，完成后每次请求都会通过有效节点进行访问。
* 修复了lssues中因trojan节点解析错误导致程序报错的问题。
* 程序结束后，不用再手动关闭clash进行，将会跟随程序一起关闭。
* 代理结果实时显示befree命令行。
* linux版问题同步修复完成。

## 编码环境

.net8


## 利用方法

```
  -f      Specify a file path						        指定存在订阅的txt文件
  -p      Specify a port number(http&socks5) 		指定代理监听端口
  -t      Specify a link for speed testing      指定一个用于测速的链接(默认https://www.google.com)
  -y      Specify a yourself clash yaml file 		单独制定自己yaml格式的配置文件(前提自己要先写好)
```



## 利用效果

#### 运行

![image-20241127140957204](./assets/image-20241127140957204.png)

#### 指定订阅文件，指定监听端口 查询节点

![image-20241127164212711](./assets/image-20241127164212711.png)

#### 目录扫描

调用dirsx进行扫描效果

![image-20241127150550981](./assets/image-20241127150550981.png)

#### 指定自己的配置文件

![image-2222222](./assets/image-2222222.png)

## 当前支持协议

| 协议类型 | 是否支持               |
| -------- | ---------------------- |
| vmess    | 支持√                  |
| trojan   | 支持√                  |
| ss       | 支持√                  |
| ssr      | 解析存在问题，下次更新 |

