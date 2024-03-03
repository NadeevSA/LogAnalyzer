import { useEffect, useState } from 'react';
import './App.css';
import { Theme, presetGpnDefault } from '@consta/uikit/Theme';
import { Button } from '@consta/uikit/Button';
import { TextField } from '@consta/uikit/TextField';
import { Card } from '@consta/uikit/Card';
import { Text } from '@consta/uikit/Text';
import ComponentDemo from './Table'
import { Grid, GridItem } from '@consta/uikit/Grid';
import { Header, HeaderModule } from '@consta/uikit/Header';
import { Loader } from '@consta/uikit/Loader';
import { TabsExample } from './Tabs.js'
import { TableGitResult } from './TableGitResult.js'
import { FileField } from '@consta/uikit/FileField';

function App() {
  const [result, setResult] = useState();
  const [btnLoad, setBtnLoad] = useState(false);
  const [logger, setLogger] = useState('_logger')
  const [repository, setRepository] = useState()
  const [checkFiles, setCheckFiles] = useState([])
  const [nameSln, setNameSln] = useState('GpnDs.UBER.sln')
  const [tab, setTab] = useState('Настройки')

  function fetchCalculation() {
    setTab('Результаты изменений')
    setBtnLoad(true)
    return fetch('http://localhost:5027/api/Core/Calculation', {
      method: 'POST',
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({nameLogger: logger, 
                            checkFiles: checkFiles, 
                            path: repository.path,
                            nameFolder: repository.nameFolder,
                            ifElseChecker: true,
                          }),
    })
    .then(response => {
        console.log(response)
        return response.json()
    })
    .then(data => {
        console.log(data)
        setResult(data)
        setBtnLoad(false)
    })
    .catch(error => {
      console.warn(error)
      setBtnLoad(false)
    });
  }

  function fetchPostRepository() {
    setBtnLoad(true)
    return fetch('http://localhost:5027/api/Git/Repository/', {
      method: 'POST',
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({nameSln: nameSln }),
    })
    .then(response => {
        console.log(response)
        return response.json()
    })
    .then(data => {
        setRepository(data)
        setBtnLoad(false)
    })
    .catch(error => {
        console.warn(error)
        setBtnLoad(false)
      });
  }
  
  function f(s){
    s.map(v => console.log(v.Path))
    setCheckFiles(s.map(v => v.Path))
  }

  function x(s){
    console.log(s)
    setTab(s.label)
  }

  const onFileDownload = (e) => {
    const target = e.target
    setNameSln(target.files[0].name)
  }

  useEffect(() => {
    fetchPostRepository()
  }, [nameSln])

  return (
    <Theme preset={presetGpnDefault}>
      <Header
        leftSide={
          <>
          <HeaderModule indent="m">
            <Button disabled={!(nameSln && logger)} style={{margin: '0 25px 0 0'}} view="secondary" label="Старт анализа" onClick={() => 
              {
                fetchCalculation()
              }} loading={btnLoad}/>
          </HeaderModule>
          <TabsExample x={x} value={tab}></TabsExample>
          </>
        }
      />
        { tab == "Настройки" &&
        <Grid gap="xl" cols={4}>
          <GridItem col={1}>
            <Card verticalSpace="l" horizontalSpace="l" style={{height: '87vh', margin: '3vh 0 0 3vh'}}>
              <FileField onChange={onFileDownload} >
                {(props) => <Button {...props} label="Выбрать solution" view="secondary"/>}
              </FileField>
              <Text style={{margin: '1vh 0 0 0'}}>Название .sln файла</Text>
              <TextField
                    value={nameSln}
                    type="textarea"
                    cols={1000}
                    placeholder="solution" />
              <Text style={{margin: '1vh 0 0 0'}}>Название логгера в коде</Text>
              <TextField
                    onChange={(event) => setLogger(event.value)}
                    value={logger}
                    type="textarea"
                    cols={1000}
                    placeholder="Название логгера" />
            </Card>
          </GridItem>
          <GridItem col={3}>
            <Card verticalSpace="l" horizontalSpace="l" style={{height: '87vh', margin: '3vh 3vh 0 0'}}>
              { !btnLoad ? 
                <ComponentDemo data={repository?.hierarchyFilesJson} f={f}/> : <Loader></Loader>
              }
            </Card>
          </GridItem>
          </Grid>
        }
        { tab == "Результаты изменений" &&
        <Grid gap="xl" cols={3}>
          <GridItem col={3}>
            <Card verticalSpace="l" horizontalSpace="l" style={{height: '87vh', margin: '3vh 3vh 0 0'}}>
              <h4>Общее количество измененных логов: {result ? result.allCountLoggers : 0}</h4>
              <h4>Время работы: {result ? result?.timeWork : 0} сек.</h4>
              <TableGitResult data={result?.changeLoggersJson} ></TableGitResult>
            </Card>
          </GridItem>
        </Grid>
        }
    </Theme>
  );
}

export default App;
