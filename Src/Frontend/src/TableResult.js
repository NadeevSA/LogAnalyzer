import {  useEffect, useState  } from 'react';
import {  Table  } from 'antd';

const columns = [
  {
    title: 'Путь файла',
    dataIndex: 'FilePath',
  },
  {
    title: 'Номер строки',
    dataIndex: 'LineNumber',
  },
  {
    title: 'Код',
    dataIndex: 'Result',
  },
  {
    title: 'Результат',
    dataIndex: 'ResultText',
  }
];

export const TableResult = (props) => {
  const [checkStrictly, setCeckStrictly] = useState(false);
  const [json, setJson] = useState([]);
  
  useEffect(() => {
    //setSelectedRowKeys(json.map(i => i.key))
  }, [json])

  useEffect(() => {
    if (props.data)
    {
      setJson(props?.data? JSON.parse(props.data) : [])
    } 
  }, [props?.data])

  const rowSelection = {
    onChange: (_, selectedRows) => {
      //props.f(selectedRows)
    }
  };

  return (
    <>
      <Table
        pagination={false}
        columns={columns}
        size='small'
        //expandable={{ defaultExpandAllRows: true }}
        rowSelection={{
          ...rowSelection,
          checkStrictly,
          //defaultSelectedRowKeys: selectedRowKeys,
        }}
        dataSource={json}
      />
    </>
  );
};
