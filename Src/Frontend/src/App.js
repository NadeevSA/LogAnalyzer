import { useState, useEffect } from 'react';
import './App.css';
import { Theme, presetGpnDefault } from '@consta/uikit/Theme';
import { Pie } from '@consta/charts/Pie';
import { Button } from '@consta/uikit/Button';
import { TextField } from '@consta/uikit/TextField';
import { Stats } from '@consta/stats/Stats';
import { Card } from '@consta/uikit/Card';
import { Text } from '@consta/uikit/Text';
import { Select } from '@consta/uikit/Select';
import ComponentDemo from './Table'
import { Grid, GridItem } from '@consta/uikit/Grid';
import { Header, HeaderModule } from '@consta/uikit/Header';
import { Loader } from '@consta/uikit/Loader';
import { Switch } from '@consta/uikit/Switch';
import { TabsExample } from './Tabs.js'
import { Informer } from '@consta/uikit/Informer';
import { TableResult } from './TableResult.js'

function App() {
  const [result, setResult] = useState();
  const [btnLoad, setBtnLoad] = useState(false);
  const [logger, setLogger] = useState('logger')
  const [branches, setBranches] = useState([]);
  const [value, setValue] = useState();
  const [repository, setRepository] = useState()
  const [checkFiles, setCheckFiles] = useState([])
  const [repo, setRepo] = useState('https://github.com/NadeevSA/TestForDiplom.git')
  const [nameSln, setNameSln] = useState('OOP')
  const [checkIfElse, setCheckIfElse] = useState(true)
  const [tab, setTab] = useState('Настройки')

  function fetchCalculation() {
    setBtnLoad(true)
    return fetch('http://localhost:5027/api/Core/Calculation', {
      method: 'POST',
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({nameLogger: logger, 
                            checkFiles: checkFiles, 
                            nameBranch: value.value, 
                            path: repository.path,
                            ifElseChecker: checkIfElse,
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

  function fetchPostBranches() {
    return fetch('http://localhost:5027/api/Core/BranchesByNameRepo', {
      method: 'POST',
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({nameBranch: '', nameRepo: repo, nameSln: nameSln?.value }),
    })
    .then(response => {
        console.log(response)
        return response.json()
    })
    .then(data => {
        setBranches(data)
    })
    .catch(error => {
      console.warn(error)
    });
  }

  function fetchPostRepository() {
    return fetch('http://localhost:5027/api/Core/Repository/', {
      method: 'POST',
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({nameBranch: value?.value, nameRepo: repo, nameSln: nameSln }),
    })
    .then(response => {
        console.log(response)
        return response.json()
    })
    .then(data => {
        setRepository(data)
    })
    .catch(error => console.warn(error));
  }

  const data = [
    { type: 'Без логов', value: 100 - result?.percentageLogs },
    { type: 'С логами', value: result?.percentageLogs },
  ];
  
  function f(s){
    s.map(v => console.log(v.Path))
    setCheckFiles(s.map(v => v.Path))
  }

  function x(s){
    console.log(s)
    setTab(s.label)
  }

  return (
    <Theme preset={presetGpnDefault}>
      <Header
        leftSide={
          <>
          <HeaderModule indent="m">
            <Button disabled={!(repository && value && nameSln && logger)} style={{margin: '0 25px 0 0'}} view="secondary" label="Старт анализа" onClick={() => 
              {
                fetchCalculation()
              }} loading={btnLoad}/>
          </HeaderModule>
          <TabsExample x={x}></TabsExample>
          </>
        }
      />
        { tab == "Настройки" &&
        <Grid gap="xl" cols={3}>
          <GridItem col={1}>
            <Card verticalSpace="l" horizontalSpace="l" style={{height: '87vh', margin: '3vh 0 0 3vh'}}>
              <Text style={{margin: '1vh 0 0 0'}}>Репозиторий</Text>
              <TextField
                    onChange={(event) => setRepo(event.value)}
                    value={repo}
                    type="textarea"
                    cols={1000}
                    placeholder="Название репозитория"
              />
              <Text style={{margin: '1vh 0 0 0'}}>Ветка</Text>
              <Select
                width={5}
                placeholder="Выберите ветку"
                items={branches?.map(d => {return {id: d, label: d}}) ?? []}
                onClick={() => fetchPostBranches()}
                value={value}
                onChange={d => {
                  setValue(d.value)
                  fetchPostRepository()
                }}
              />
              <Text style={{margin: '1vh 0 0 0'}}>Название логгера в коде</Text>
              <TextField
                    onChange={(event) => setLogger(event.value)}
                    value={logger}
                    type="textarea"
                    cols={1000}
                    placeholder="Название логгера"
                    />
              <Text style={{margin: '1vh 0 0 0'}}>Solution</Text>
              <TextField
                    onChange={(event) => setNameSln(event.value)}
                    value={nameSln}
                    type="textarea"
                    cols={1000}
                    placeholder="Название Solution"
                    />
              <Switch 
                    style={{margin: '3vh 0 0 0'}}
                    size="l" 
                    label="IfElse" 
                    checked={checkIfElse}
                    onChange={() => setCheckIfElse(!checkIfElse)}
              />
            </Card>
          </GridItem>
          <GridItem col={2}>
            <Card verticalSpace="l" horizontalSpace="l" style={{height: '87vh', margin: '3vh 3vh 0 0'}}>
                <ComponentDemo data={repository?.hierarchyFilesJson} f={f}/>
            </Card>
          </GridItem>
          </Grid>
        }
        { tab == "Результаты" &&
        <Grid gap="xl" cols={3}>
          <GridItem col={1}>
            <Card verticalSpace="l" horizontalSpace="l" style={{height: '87vh', margin: '3vh 0 0 0'}}>
                <Stats
                  value={result?.percentageLogs ?? 0}
                  title="Процент логгов"
                  status="success" />
                <Pie
                style={{
                  width: 300,
                }}
                data={data}
                angleField="value"
                colorField="type"
                />
            </Card>
          </GridItem>
          <GridItem col={2}>
            <Card verticalSpace="l" horizontalSpace="l" style={{height: '87vh', margin: '3vh 3vh 0 0'}}>
                <TableResult data={repository?.hierarchyFilesJson} f={f}/>
            </Card>
          </GridItem>
        </Grid>
        }
    </Theme>
  );
}

export default App;
