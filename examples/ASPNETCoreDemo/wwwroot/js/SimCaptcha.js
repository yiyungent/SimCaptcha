(function () {
	// 私有字段
	// 点击此 _element 元素则弹出验证码层，为此元素绑定点击事件
	var _element = null;
	var _appId = "";
	// 前端验证成功后，会调用业务传入的回调函数，并在第一个参数中传入回调结果
	var _callback = null;
	var _options = null; // 保留，暂时无用

	// 用户点击验证码图片的位置数据 {Array} eg:  [{ x: 12, y: 35 }, { x: 52, y: 35 }, { x: 32, y: 75 }]
	var _vCodePos = [];

	// 验证码服务端 效验url
	var _reqVCodeCheckUrl = "";
	// 验证码服务端 获取验证码图片url
	var _reqVCodeImgUrl = "";

	// 从后端响应得到的appId
	var _resAppId = "";
	// 验证是否通过效验票据
	var _resTicket = "";
	// 用户会话唯一标识
	var _resUserId = "";
	// 验证码图片 base64
	var _resVCodeImg = "";
	// 验证码效验秘钥
	var _resVCodeKey = "";
	// 验证码提示 eg: 请依序点击 走 炮 跳
	var _resVCodeTip = "";

	// 错误提示 eg: 1.点错啦，请重试 2.这题有点难，为你换一个试试吧
	var _errorTip = "";

	/***
	 * 隐藏当前验证码弹出层，下次show 将使用当前验证码图片base64
	 * 用于用户手动点击关闭按钮
	 */
	function hidden() {
		// TODO: 需DOM操作
	}

	/***
	 * 摧毁当前验证码（隐藏验证码弹出层，清除验证码图片base64），下次show 将请求新验证码图片base64
	 */
	function destroy() {
		// 隐藏当前验证码弹出层
		hidden();
		// 清除全部点触标记
		clearPointMark();
		// 清空点触位置数据
		_vCodePos = [];
		// 清除验证码相关数据
		_resVCodeImg = "";
		_resVCodeKey = "";
		_resVCodeTip = "";
		_resAppId = "";
		_resTicket = "";

		_errorTip = "";
	}



	/**
	 * 在验证码弹出层展示 成功通过验证
	 * @param {Number} ts 本次点击验证码花费时间（js 13位时间戳）// 保留，暂时不用，随便传一个，或不传
	 */
	function showSuccessTip(ts) {
		// TODO: 在验证码弹出层展示 成功通过验证
		// TODO: 需DOM操作
	}

	/**
	 * 清空图片上的全部点触标记
	 */
	function clearPointMark() {
		// TODO: 找到 .simCaptcha 内的 所有 .simCaptcha_mark 元素 -> 全部移除
		// TODO: 需DOM操作
	}

	/***
	 * 将用户点击验证码的位置数据发送到验证码服务端   (每个位置(x轴, y轴))
	 * @param vCodePos {Array} eg: ["12,35", "134,656", "133,44", "33,13"]
	 * @param vCodeKey {String} 验证码效验秘钥
	 */
	function sendVCodePos(vCodePos, vCodeKey) {
		var ts = Date.now(); // js 13位 毫秒时间戳
		var verifyInfo = { vcodePos: vCodePos, vCodeKey: vCodeKey, userId: _resUserId, ua: navigator.userAgent, ts: ts }; // ua, ts 服务端暂时未用，保留。用户花费在此验证码的时间 = 验证码服务端 接收到点击位置数据时间 - 验证码服务端 产生验证码图片时间
		// 发送ajax到验证码服务端 -> 得到response结果，封装为 res
		httpPost(_reqVCodeCheckUrl, verifyInfo, function (response) {

			console.log("sendVCodePos", response);

			// code: 0: 通过验证
			if (response.code == 0) {
				// 通过验证 -> 1.回调callback（成功回调） 2.销毁验证码弹出层destroy
				var res = { ret: 0, ticket: response.data.ticket, appId: response.data.appId, bizState: null }; // bizState自定义透传参数，暂未实现，保留
				// 将 从验证码服务端得到的 appId, ticket存起来
				_resAppId = res.appId;
				_resTicket = res.ticket;
				_callback(res);
				// 在摧毁验证码层之前，先在验证码层展示成功通过验证提示
				showSuccessTip();
				destroy();
			} else {
				// 未通过验证 -> 1.提示用户 2.if(错误次数未达上限)：清空用户点击验证码的位置数据，重置，让用户重新点击 3.else(错误次数达到上限)：刷新验证码弹出层（请求新验证码图片，更新验证码提示）
				// code: -1: 验证码错误 且 错误次数未达上限
				if (response.code == -1) {
					_errorTip = "点错啦, 请重试";
					// 清空点触位置数据
					_vCodePos = [];
					// 清除图片上的全部点触标记
					clearPointMark();
					// 更新验证码效验秘钥
					_resVCodeKey = response.data.vCodeKey;
				} else if (response.code == -2) {
					// code: -2: 验证码错误 且 错误次数已达上限
					_errorTip = "这题有点难, 为你换一个试试吧";
					refreshVCode();
				} else if (response.code == -3) {
					// 验证码无效（被篡改）
					_errorTip = "验证码无效, 为你换一个试试吧";
					refreshVCode();
				} else if (response.code == -4) {
					// 验证码过期
					_errorTip = "验证码过期, 为你换一个试试吧";
					refreshVCode();
				} else if(response.code == -5) {
					// 验证码无效
					_errorTip = "验证码无效, 为你换一个试试吧";
					refreshVCode();
				} else if(response.code == -6) {
					// 验证码无效
					_errorTip = "验证码无效, 为你换一个试试吧";
					refreshVCode();
				}

			}


		});


	}

	/***
	 * 刷新验证码弹出层：1.刷新验证码图片，2.更新验证码提示 3. 清空点触位置数据 4.清空图片上的全部点触标记
	 */
	function refreshVCode() {
		// 清空点触位置数据
		_vCodePos = [];
		// 清除图片上的全部点触标记
		clearPointMark();
		// ajax请求新的验证码图片base64
		httpGet(_reqVCodeImgUrl, function (response) {
			if (response.code == 0) {
				// 成功获取新验证码
				// 保存并更新 验证码图片
				_resVCodeImg = response.data.vCodeImg;
				// 保存验证码效验秘钥
				_resVCodeKey = response.data.vCodeKey;
				// 更新验证码提示
				_resVCodeTip = response.data.vCodeTip;
			} else {
				// 获取验证码失败
				_errorTip = response.message;
			}
		});
	}

	/***
	 * 手动初始化并绑定到一个元素
	 * @param element {HTMLElement} 需要绑定click事件的元素（注意：手动绑定不要使用id="SimCaptcha"的元素，避免重复绑定点击）
	 * @param appId {String} 申请的场景Id
	 * @param callback {Function} 回调函数
	 * @param options {Object} 更多配置参数, 详见配置参数
	 */
	function SimCaptcha(element, appId, callback, options) {
		_element = element;
		_appId = appId;
		_callback = callback;
		_options = options;
	}
	SimCaptcha.prototype = {
		constructor: SimCaptcha,

		/**
		 * 设置验证码服务端URL
		 * @param {String} reqVCodeImgUrl 验证码服务端 图片Url
		 * @param {String} reqVCodeCheckUrl 验证码服务端 效验Url
		 */
		setUrl: function (reqVCodeImgUrl, reqVCodeCheckUrl) {
			_reqVCodeImgUrl = reqVCodeImgUrl;
			_reqVCodeCheckUrl = reqVCodeCheckUrl;
		},

		/***
		 * 显示验证码
		 */
		show: function () {
			// TODO: 显示验证码弹出层
			// TODO: 需DOM操作
		},

		/***
		 * 隐藏当前验证码弹出层，下次show 将使用当前验证码图片base64
		 * 用于用户手动点击关闭按钮
		 */
		hidden: hidden,

		/***
		 * 摧毁当前验证码（隐藏验证码弹出层，清除验证码图片base64），下次show 将请求新验证码图片base64
		 */
		destroy: destroy,

		/***
		 * return: Object:{"appId":"","ticket":""}
		 */
		getTicket: function () {
			return {
				appId: _resAppId,
				ticket: _resTicket
			};
		},



	}

	/**
	 * 发送http GET
	 * @param {String} url 请求url
	 * @param {Function} callback 请求成功回调函数
	 */

	function httpGet(url, callback) {
		// XMLHttpRequest对象用于在后台与服务器交换数据   
		var xhr = new XMLHttpRequest();
		xhr.open('GET', url, true);
		xhr.onreadystatechange = function () {
			// readyState == 4说明请求已完成
			if (xhr.readyState == 4 && xhr.status == 200 || xhr.status == 304) {
				// 从服务器获得数据 
				callback.call(this, xhr.responseText);
			}
		};
		xhr.send();
	}

	/**
	 * 发送http POST
	 * @param {String} url 请求url
	 * @param {Ojbect} data 将要发送的数据包装为对象
	 * @param {Function} callback 请求成功回调函数
	 */
	function httpPost(url, data, callback) {
		var xhr = new XMLHttpRequest();
		xhr.open("POST", url, true);
		// 添加http头，发送信息至服务器时内容编码类型
		xhr.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
		xhr.onreadystatechange = function () {
			if (xhr.readyState == 4 && (xhr.status == 200 || xhr.status == 304)) {
				callback.call(this, xhr.responseText);
			}
		};
		xhr.send(JSON.stringify(data));
	}




	//===========================================================================

	//======
	// NODE
	//======
	if (typeof exports !== 'undefined') {
		if (typeof module !== 'undefined' && module.exports) {
			exports = module.exports = SimCaptcha;
		}
		exports.StateMachine = SimCaptcha;
	}
	//============
	// AMD/REQUIRE
	//============
	else if (typeof define === 'function' && define.amd) {
		define(function (require) { return SimCaptcha; });
	}
	//========
	// BROWSER
	//========
	else if (typeof window !== 'undefined') {
		window.SimCaptcha = SimCaptcha;
	}
	//===========
	// WEB WORKER
	//===========
	else if (typeof self !== 'undefined') {
		self.SimCaptcha = SimCaptcha;
	}

}());