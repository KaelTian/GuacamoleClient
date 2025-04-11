<template>
 <!-- 固定尺寸容器 -->
 <div id="display-container">
    <div id="display"></div>
  </div>
</template>

<script setup>
import { onMounted,onUnmounted } from 'vue';
import Guacamole from 'guacamole-common-js';
import { ref } from 'vue';
import { getTokens } from '../common/api';

const client = ref(null);
const identifier = ref('');
const connect = async () => {
  try {  // 从URL参数中获取值
    const token = await getTokens();
    const urlParams = new URLSearchParams(window.location.search);
    identifier.value = urlParams.get('identifier');

    // 1. 创建客户端（不再传递动态尺寸）
    client.value = new Guacamole.Client(
      new Guacamole.WebSocketTunnel('ws://192.168.0.209:8080/guacamole/websocket-tunnel')
    );

    // 2. 挂载显示层
    const display = document.getElementById('display');
    display.innerHTML = '';
    display.appendChild(client.value.getDisplay().getElement());

    // 3. 设置固定分辨率参数（与容器尺寸一致）
    const displayContainer = document.getElementById('display');
    const width = displayContainer.offsetWidth;
    const height = displayContainer.offsetHeight;
    const params = new URLSearchParams({
        GUAC_DATA_SOURCE: token?.dataSource,
        GUAC_ID: identifier.value,
        GUAC_TYPE: 'c',
        GUAC_WIDTH: width.toString(),  // 必须与CSS容器宽度一致
        GUAC_HEIGHT: height.toString(), // 必须与CSS容器高度一致
        GUAC_DPI: '96'
    });

    // 4. 音频/图像支持（示例）
    ['audio/L8', 'audio/L16'].forEach(audio => params.append('GUAC_AUDIO', audio));
    ['image/jpeg', 'image/png'].forEach(image => params.append('GUAC_IMAGE', image));

    // 4. 连接
    client.value.connect(`token=${token.authToken}&${params.toString()}`);

    // 5. 精准鼠标事件处理（关键！）
    const displayElement = client.value.getDisplay().getElement();
    const mouse = new Guacamole.Mouse(displayElement);

    mouse.onEach(['mousedown', 'mouseup', 'mousemove'], function sendMouseEvent(e) {
        client.value.sendMouseState(e.state);
    });
    // 键盘事件注册
    var keyboard = new Guacamole.Keyboard(document);
    keyboard.onkeydown = function (keysym) {
        client.value.sendKeyEvent(1, keysym);
    };
    keyboard.onkeyup = function (keysym) {
        client.value.sendKeyEvent(0, keysym);
    };
  } catch (error) {
    console.error('token error: ', error.response ? error.response.status : 'unknown', error.response ? error.response.statusText : 'unknown');
  }
};



onMounted(() => {
  connect();
});

onUnmounted(() => {
    if (client.value) client.value.disconnect();
});

</script>

<style scoped>
html, body {
    margin: 0;
    padding: 0;
    overflow: hidden;
}
#display-container {
    position: absolute;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    margin: 0;
    padding: 0;
    overflow: hidden;
}
  
  #display {
    width: 100%;
    height: 100%;
  }
  
</style>