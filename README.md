# BAO
For BUPT  
项目网络框架灵感来源https://zhuanlan.zhihu.com/p/39005219  
整体架构：
![image](https://github.com/chaozuoye/BAO/blob/master/%E6%95%B4%E4%BD%93%E6%A1%86%E6%9E%B6.png)  
服务端和客户端使用统一消息结构进行信息收发。  
客户端架构
 ![image](https://github.com/chaozuoye/BAO/blob/master/%E5%AE%A2%E6%88%B7%E7%AB%AF%E6%A1%86%E6%9E%B6.png)  
 客户端分为三层结构，其中还附带有各种工具类。这三层分别是：  
 Network层，负责接收和发送数据；  
 Player层，一个客户端只有一个Player，Player层作为Network层和Gamer层的中间层负责上下层之间的调度。  
 Gamer层，一个客户端可以有多个Gamer但只有一个本地Gamer，除本地Gamer外其他Gamer只能接受Player的调度而不能向上调度其他层。Gamer层主要负责人物动作逻辑以及音频录制播放。
