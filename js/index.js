$(function() {

	// chat aliases
	var you = 'You';
	var robot = 'JBot';
	
	// slow reply by 400 to 800 ms
	var delayStart = 400;
	var delayEnd = 800;
	
	// initialize 
	var chat = $('.chat');
	var waiting = 0;
	$('.busy').text(robot + ' is typing...');

	// submit user input and get chat-bot's reply
	var submitChat = function() {
	
		var input = $('.input input').val();
		if(input == '') return;
		var requestData={"q":input};
		console.log(requestData);
		
		$('.input input').val('');
		updateChat(you, input);
		$.ajax({
        url:"http://localhost/test/aichat/app.php",  // request page url
        type: "POST",
        dataType: "json",
        crossDomain: true,
        data: requestData
		})
        .done(function (data) {
			
			console.log(requestData);
			console.log(data);
			
			reply = data.answer;  
			if(reply == null || reply.length==0) return;
			
			var latency = Math.floor((Math.random() * (delayEnd - delayStart)) + delayStart);
			$('.busy').css('display', 'block');
			waiting++;
			setTimeout( function() {
				if(typeof reply === 'string') {
					updateChat(robot, reply);
				} else {
					for(var r in reply) {
						updateChat(robot, reply[r]);
					}
				}
				if(--waiting == 0) $('.busy').css('display', 'none');
			}, latency);
        })
        .fail(function () {
        });
		
	}
	
	// add a new line to the chat
	var updateChat = function(party, text) {
	
		var style = 'you';
		var htmlChat = ""
				+'<div class="jchat_user_message">'
				+'	<div class="jchat_message_content poyon">'
				+'		<div class="right_triangle poyon"></div>'
				+'		<span class="party"></span> <span class="text"></span>'
				+'	</div>'
				+ '<p class="jchat_time_stamp"></p>'
				+'</div>';
				
		if(party != you) {
			style = 'other';
			htmlChat = ""
				+'<div class="jchat_message">'
				+'	<img src="img/bot-icon.png" class="jchat_user_icon">'
				+'	<div class="jchat_message_content">'
				+'		<div class="left_triangle"></div>'
				+'		<span class="party"></span> <span class="text"></span>'
				+'	</div>'
				+ '<p class="jchat_time_stamp"></p>'
				+'</div>';
		}
		
		//var line = $('<div><span class="party"></span> <span class="text"></span></div>');
		var line = $(htmlChat);
		line.find('.party').addClass(style).text(party + ':');
		line.find('.text').html(text);
		line.find('p.jchat_time_stamp').text(currentTime());
		
		chat.append(line);
		
		chat.stop().animate({ scrollTop: chat.prop("scrollHeight")});
	
	}
	
	var currentTime = function() {
		var now = new Date();
		var year = now.getFullYear();
		var mon = now.getMonth()+1; //１を足すこと
		var day = now.getDate();
		var hour = now.getHours();
		var min = now.getMinutes();
		var sec = now.getSeconds();

		//出力用
		var result = year + "年" + mon + "月" + day + "日"  + " "+ hour + "時" + min + "分" + sec + "秒"; 
		return result;
	}
	
	// event binding
	$('.input').bind('keydown', function(e) {
		if(e.keyCode == 13) {
			submitChat();
		}
	});
	
	$('.input a').bind('click', submitChat);
	

	
	
	// initial chat state
	updateChat(robot, 'チャットのご利用ありがとうございます。<br>AIサポートのJチャットです。');

});