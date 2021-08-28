/*
 * @Author: yiyun
 * @Description:
 */
module.exports = {
  title: "SimCaptcha",
  description: "一个简单易用的点触验证码",
  base: "/SimCaptcha/",
  themeConfig: {
    // 假定是 GitHub. 同时也可以是一个完整的 GitLab URL
    repo: "yiyungent/SimCaptcha",
    // 自定义仓库链接文字。默认从 `themeConfig.repo` 中自动推断为
    // "GitHub"/"GitLab"/"Bitbucket" 其中之一，或是 "Source"。
    repoLabel: "GitHub",

    // 以下为可选的编辑链接选项

    // 假如你的文档仓库和项目本身不在一个仓库：
    //docsRepo: 'vuejs/vuepress',
    // 假如文档不是放在仓库的根目录下：
    docsDir: "docs",
    // 假如文档放在一个特定的分支下：
    docsBranch: "master",
    // 默认是 false, 设置为 true 来启用
    editLinks: true,
    // 默认为 "Edit this page"
    editLinkText: "帮助我们改善此页面！",
    lastUpdated: "Last Updated", // string | boolean

    nav: [
      { text: "首页", link: "/" },
      { text: "指南", link: "/Guide/" },
      {
        text: "SimCaptcha",
        items: [
          { text: "SimCaptcha", link: "/SimCaptcha/" },
          { text: "SimCaptcha.AspNetCore", link: "/SimCaptcha.AspNetCore/" },
        ],
      },
      {
        text: "Web SDK",
        items: [
          { text: "sim-captcha-js", link: "/WebSDK/sim-captcha-js/" },
          { text: "vue-sim-captcha", link: "/WebSDK/vue-sim-captcha/" },
        ],
      },
    ],

    sidebarDepth: 2,
    sidebar: {
      "/Guide/": [
        {
          title: "指南", // 必要的
          path: "/Guide/",
          collapsable: false, // 可选的, 默认值是 true,
          children: [
            {
              title: "依赖",
              path: "dependence",
            },
          ],
        },
      ],
      "/SimCaptcha/": [
        {
          title: "SimCaptcha", // 必要的
          path: "/SimCaptcha/",
          collapsable: false, // 可选的, 默认值是 true,
          // children: [{ title: "API", path: "api" }],
        },
      ],
      "/SimCaptcha.AspNetCore/": [
        {
          title: "SimCaptcha.AspNetCore", // 必要的
          path: "/SimCaptcha.AspNetCore/",
          collapsable: false, // 可选的, 默认值是 true,
          // children: [{ title: "API", path: "api" }],
        },
      ],
      "/WebSDK/sim-captcha-js/": [
        {
          title: "sim-captcha-js", // 必要的
          path: "/WebSDK/sim-captcha-js/",
          collapsable: false, // 可选的, 默认值是 true,
          children: [{ title: "API", path: "api" }],
        },
      ],
      "/WebSDK/vue-sim-captcha/": [
        {
          title: "vue-sim-captcha", // 必要的
          path: "/WebSDK/vue-sim-captcha/",
          collapsable: false, // 可选的, 默认值是 true,
          children: [{ title: "API", path: "api" }],
        },
      ],
    },
  },
};
