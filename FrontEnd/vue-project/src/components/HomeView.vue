<template>
    <div class="home-view">
      <div v-for="connection in connections" :key="connection.identifier" class="connection-card">
        <h2>{{ connection.name }}</h2>
        <p><strong>Identifier:</strong> {{ connection.identifier }}</p>
        <p><strong>Parent Identifier:</strong> {{ connection.parentIdentifier }}</p>
        <p><strong>Protocol:</strong> {{ connection.protocol }}</p>
        <p><strong>Active Connections:</strong> {{ connection.activeConnections }}</p>
        <p><strong>Last Active:</strong> {{ connection.lastActive }}</p>
        <h3>Attributes</h3>
        <ul>
          <li><strong>Guacd Encryption:</strong> {{ connection.attributes["guacd-encryption"] }}</li>
          <li><strong>Failover Only:</strong> {{ connection.attributes["failover-only"] }}</li>
          <li><strong>Weight:</strong> {{ connection.attributes["weight"] }}</li>
          <li><strong>Max Connections:</strong> {{ connection.attributes["max-connections"] }}</li>
          <li><strong>Guacd Hostname:</strong> {{ connection.attributes["guacd-hostname"] }}</li>
          <li><strong>Guacd Port:</strong> {{ connection.attributes["guacd-port"] }}</li>
          <li><strong>Max Connections Per User:</strong> {{ connection.attributes["max-connections-per-user"] }}</li>
        </ul>
        <button @click="openConnectionView(connection)">Open Connection</button>
      </div>
    </div>
  </template>
  
  <script setup>
  import { onMounted } from 'vue';
  import { ref } from 'vue';
  import { getTree } from '../common/api';
  
  const connections = ref([]);
  
  const openConnectionView = (connection) => {
    const params = {
      identifier: connection.identifier
    };
    
    const queryString = Object.keys(params)
      .map(key => `${encodeURIComponent(key)}=${encodeURIComponent(params[key])}`)
      .join('&');
    
    window.open(`/connectionview?${queryString}`, '_blank');
  };
  
  const fetchConnections = async () => {
    const tree = await getTree();
    if (tree) {
      connections.value = tree.childConnections;
    }
  };
  
  onMounted(() => {
    fetchConnections();
  });
  </script>
  
  <style scoped>
  .home-view {
    padding: 20px;
  }
  .connection-card {
    border: 1px solid #ddd;
    border-radius: 8px;
    padding: 20px;
    margin-bottom: 20px;
    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  }
  .connection-card h2 {
    margin-top: 0;
  }
  .connection-card ul {
    list-style-type: none;
    padding: 0;
  }
  .connection-card li {
    margin-bottom: 5px;
  }
  .connection-card button {
    background-color: #007BFF;
    color: white;
    border: none;
    padding: 10px 20px;
    border-radius: 5px;
    cursor: pointer;
  }
  .connection-card button:hover {
    background-color: #0056b3;
  }
</style>