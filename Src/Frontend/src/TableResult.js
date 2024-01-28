import {  useEffect, useState  } from 'react';
import {  Table  } from 'antd';

const columns = [
  {
    title: 'Репозито1sрий',
    dataIndex: 'FileName',
  },
];

// rowSelection objects indicates the need for row selection
export const TableResult = (props) => {
  const [checkStrictly, setCeckStrictly] = useState(false);
  const [selectedRowKeys, setSelectedRowKeys] = useState([]);
  const [json, setJson] = useState([]);
  
  useEffect(() => {
    setSelectedRowKeys(json.map(i => i.key))
  }, [json])

  useEffect(() => {
    setJson(props?.data? JSON.parse(props.data) : [])
  }, [props?.data])

  const rowSelection = {
    onChange: (_, selectedRows) => {
      props.f(selectedRows)
    }
  };

  return (
    <>
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
      />
    </>
  );
};
