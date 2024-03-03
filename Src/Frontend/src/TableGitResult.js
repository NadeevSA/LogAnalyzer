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
  }
];

export const TableGitResult = (props) => {
  const [checkStrictly, setCeckStrictly] = useState(false);
  const [json, setJson] = useState([]);
  
  useEffect(() => {
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
        rowSelection={{
          ...rowSelection,
          checkStrictly,
        }}
        dataSource={json}
      />
    </>
  );
};
