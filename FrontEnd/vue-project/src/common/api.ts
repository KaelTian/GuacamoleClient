import axios from 'axios';

// 定义getToken函数返回的数据结构类型
type TokenResponse = {
  authToken: string;
  username: string;
  dataSource: string;
  availableDataSources: string[];
};

// 获取tokens
const getTokens = async (): Promise<TokenResponse | null> => {
  try {
    const response = await axios.post('/guacamole/api/tokens', `username=guacadmin&password=guacadmin`, {
      headers: {
        'Content-Type': 'application/x-www-form-urlencoded'
      }
    });
    return response.data;
  } catch (error) {
    console.error('token error: ', error.response ? error.response.status : 'unknown', error.response ? error.response.statusText : 'unknown');
    return null;
  }
};

// 定义getTree函数返回的数据结构类型
type TreeResponse = {
    name: string;
    identifier: string;
    type: string;
    activeConnections: number;
    childConnections: Array<{
        name: string;
        identifier: string;
        parentIdentifier: string;
        protocol: string;
        attributes: {
            "guacd-encryption": null | string;
            "failover-only": null | string;
            "weight": null | string;
            "max-connections": string;
            "guacd-hostname": null | string;
            "guacd-port": null | string;
            "max-connections-per-user": string;
        };
        activeConnections: number;
        lastActive: number;
    }>;
    attributes: {};
};

// 获取tree
const getTree = async (): Promise<TreeResponse | null> => {
  try {
    const token = await getTokens();
    const response = await axios.get(`/guacamole/api/session/data/${token?.dataSource}/connectionGroups/ROOT/tree`, {
      headers: {
        'Guacamole-Token': token?.authToken
      }
    });
    return response.data;
  } catch (error) {
    console.error('get tree error: ', error.response ? error.response.status : 'unknown', error.response ? error.response.statusText : 'unknown');
    return null;
  }
};

export {
  getTokens,
  getTree
};