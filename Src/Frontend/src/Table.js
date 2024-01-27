import {  useEffect, useState  } from 'react';
import {  Table  } from 'antd';

const columns = [
  {
    title: 'Репозиторий',
    dataIndex: 'FileName',
  },
];

// rowSelection objects indicates the need for row selection
const App = (props) => {
  const [checkStrictly, setCeckStrictly] = useState(false);
  const [selectedRowKeys, setSelectedRowKeys] = useState([]);
  const [json, setJson] = useState([]);
  
  useEffect(() => {
    console.log("yourJsonObject " + json.map(i => i.key))
    setSelectedRowKeys(json.map(i => i.key))
  }, [json])

  useEffect(() => {
    console.log("props?.data")
    setJson(props?.data? JSON.parse(props.data) : [])
  }, [props?.data])

  const rowSelection = {
    onChange: (_, selectedRows) => {
      props.f(selectedRows)
    }
  };

  return (
    <>
    { selectedRowKeys.length > 0 ?
      <Table
        pagination={false}
        columns={columns}
        size='middle'
        //expandable={{ defaultExpandAllRows: true }}
        rowSelection={{
          ...rowSelection,
          checkStrictly,
          defaultSelectedRowKeys: selectedRowKeys,
        }}
        dataSource={json}
      /> : <Table dataSource={[]}/>
    }
    </>
  );
};
const ComponentDemo = App;

export default ComponentDemo;
