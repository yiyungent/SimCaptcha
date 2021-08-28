<!--
 * @Author: yiyun
 * @Description: 
-->
# SimCaptcha

<h1 align="center">SimCaptcha</h1>

> :cake: 一个简单易用的点触验证码, 包含了客户端与服务端实现

[![repo size](https://img.shields.io/github/repo-size/yiyungent/SimCaptcha.svg?style=flat)]()
[![LICENSE](https://img.shields.io/github/license/yiyungent/SimCaptcha.svg?style=flat)](https://github.com/yiyungent/SimCaptcha/blob/master/LICENSE)
[![nuget](https://img.shields.io/nuget/v/SimCaptcha.svg?style=flat)](https://www.nuget.org/packages/SimCaptcha/)
[![downloads](https://img.shields.io/nuget/dt/SimCaptcha.svg?style=flat)](https://www.nuget.org/packages/SimCaptcha/)

<!-- [English](README_en.md) -->

## 介绍

一个简单易用的点触验证码促进你的开发

- **简单** - 约定优于配置, 以最少的配置帮助你专注于业务.
- **易扩展** - 松散架构, 轻松扩展.
- **开箱即用** - 使用现成 Web SDK 接入后端验证

## 在线演示

- https://captcha-client.moeci.com/index.html
  - 仅供演示, 不稳定, 且非最新版, SSL 证书链尚不完整，可能在手机浏览器异常

## 前后端调用时序图

<img src="/images/time.png">

## 依赖

只需要满足下方其中一条.

- .NET Framework (>= 4.0) 被安装.
- .NET Standard (>= 2.0) 被安装.

## 安装

推荐使用 [NuGet](https://www.nuget.org/packages/SimCaptcha), 在你项目的根目录 执行下方的命令, 如果你使用 Visual Studio, 这时依次点击 **Tools** -> **NuGet Package Manager** -> **Package Manager Console** , 确保 "Default project" 是你想要安装的项目, 输入下方的命令进行安装.

```bash
PM> Install-Package SimCaptcha
```