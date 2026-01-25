**Unity TextMeshPro 描边/阴影合批优化**。  
**描边拓展**：不打断合拼前提下支持描边宽度修改，添加外描边支持。  
**阴影拓展**：不打断合批前提下支持Offset、Dilate、Softness修改。  
**优化方向**：  
  1、Outline\Underlay颜色使用R8B8G8A8，分别压缩成一个float  
  2、将RatioScalA、RatioScalB、RatioScalC、OutlieTickness压缩成一个float  
  3、将Underlay OffsetX、OffsetY、Dilate、Softness压缩成一个float  
  4、修改TextMeshPro部分源码，将以上4个float参数通过tangents（Vector4）传递给Shader  
  5、添加TMP_SDF-Mobile Vertex，在原基础上支持float解析参数，并替换对应参数  
  6、Editor Inspector拓展  
  <img width="848" height="1068" alt="C3L(45R(V(@FCN4OLL0W7B9" src="https://github.com/user-attachments/assets/3cf0a2a6-c373-4bcf-b23c-38d94778e4b5" />  
  <img width="705" height="1083" alt="B)2XA3Z4MW1NYOZ2C%0VQ~E" src="https://github.com/user-attachments/assets/1d060d60-2f31-41b4-83cd-a2a220712171" />  
  <img width="473" height="326" alt="DC1P_}6J`V6}P7UM659(7R8" src="https://github.com/user-attachments/assets/7f74d71c-9f0d-4bfe-9ed1-b9623742641a" />  
  <img width="438" height="541" alt="$G 2QI}DV 0FRACQIT%_~7T" src="https://github.com/user-attachments/assets/65ff095d-35a0-495e-aaad-911f7c0f9069" />  

**使用方式**：  
  1、TextMeshPro Material Preset选择LiberationSans SD- Vertext材质球  
  <img width="863" height="288" alt="XHCV1O2{WKP1@K192D6$CZ0" src="https://github.com/user-attachments/assets/b1c1daa6-3d56-443a-8465-d31332ab4f22" />  
  2、使用描边必须同时打开材质球以及Componet中的开关，描边类型通过材质球属性控制，其他描边参数通过Component控制。  
  <img width="849" height="1071" alt="HG83HQ6PILOVLA}NUG@U_T" src="https://github.com/user-attachments/assets/6a6d6fa0-1369-4480-98e2-e82ec2d30373" />  
  3、使用阴影类似也须同时打开两边开关，由于颜色值使用的R8B8G8A8不支持HDR故阴影内发光模式  
  <img width="846" height="1026" alt="T5 R$SWZWTU4GCKX`G~KMBM" src="https://github.com/user-attachments/assets/b9ea3e93-cdb3-4711-b03b-f84b1fc022d7" />  

**具体效果**：  
<img width="1702" height="1186" alt="d1d62ddcba10743d0f735c63bc4237c4" src="https://github.com/user-attachments/assets/2fdd5a61-a956-4d91-8bea-9353a903797b" />
