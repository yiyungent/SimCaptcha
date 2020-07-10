(function () {
	// 私有字段
	var _element = null;
	var _appId = "";
	// 前端验证成功后，会调用业务传入的回调函数，并在第一个参数中传入回调结果
	var _callback = null;
	var _options = null;

	var _vcodePos = [];

	/***
	 * 隐藏当前验证码弹出层，下次show 将使用当前验证码图片base64
	 * 用于用户手动点击关闭按钮
	 */
	function hidden() {

	}

	/***
	 * 摧毁当前验证码（隐藏验证码弹出层，清除验证码图片base64），下次show 将请求新验证码图片base64
	 */
	function destroy() {
		// 隐藏当前验证码弹出层
		hidden();
		// TODO: 清除验证码图片base64数据
	}

	/***
	 * 将用户点击验证码的位置数据发送到验证码服务端   (每个位置(x轴, y轴))
	 * @param posArr [Array] eg: ["12,35", "134,656", "133,44", "33,13"]
	 * @return
	 */
	function sendVCodePos(posArr) {
		// TODO: 发送ajax到验证码服务端 -> 得到response结果，封装为 res
		// 通过验证 -> 1.回调callback（成功回调） 2.销毁验证码弹出层destroy
		var res = {ret:0, ticket:"143jghiafbwfb8143481nfdbf", appId:"133244", bizState:null};
		_callback(res);
		destroy();

		// 未通过验证 -> 1.提示用户 2.刷新验证码弹出层（请求新验证码图片，更新验证码提示信息）
		
	}

	/***
	 * 手动初始化并绑定到一个元素
	 * @param element [HTMLElement] 需要绑定click事件的元素（注意：手动绑定不要使用id="TencentCaptcha"的元素，避免重复绑定点击）
	 * @param appId [String] 申请的场景Id
	 * @param callback [Function] 回调函数
	 * @param options [Object] 更多配置参数, 详见配置参数
	 */
	function SimCaptcha(element, appId, callback, options) {
		_element = element;
		_appId = appId;
		_callback = callback;
		_options = options;
	}
	SimCaptcha.prototype = {
		constructor: SimCaptcha,

		/***
		 * 显示验证码
		 */
		show: function () {
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
		},
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