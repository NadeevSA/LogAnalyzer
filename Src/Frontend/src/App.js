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
import { IconArrowNext } from '@consta/icons/IconArrowNext';
import { IconArrowPrevious } from '@consta/icons/IconArrowPrevious';

function App() {
  const [result, setResult] = useState();
  const [btnLoad, setBtnLoad] = useState(false);
  const [logger, setLogger] = useState('logger')
  const [branches, setBranches] = useState([]);
  const [value, setValue] = useState();
  const [repository, setRepository] = useState()
  const [checkFiles, setCheckFiles] = useState([])
  const [repo, setRepo] = useState('https://github.com/NadeevSA/TestForDiplom.git')
  const [nameSln, setNameSln] = useState('OOP.sln')
  const [checkIfElse, setCheckIfElse] = useState(true)
  const [tab, setTab] = useState('Настройки')
  const [cardResult, setCardResult] = useState(true)

  function fetchCalculation() {
    setTab('Результаты')
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
    return fetch('http://localhost:5027/api/Git/BranchesByNameRepo', {
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
    setBtnLoad(true)
    return fetch('http://localhost:5027/api/Git/Repository/', {
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
        setBtnLoad(false)
    })
    .catch(error => {
        console.warn(error)
        setBtnLoad(false)
      });
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
          <Button disabled={tab != "Результаты"} view="secondary" label="" iconRight={cardResult ? IconArrowNext : IconArrowPrevious} onClick={() => setCardResult(!cardResult)}/>
          <HeaderModule indent="m">
            <Button disabled={!(repository && value && nameSln && logger)} style={{margin: '0 25px 0 0'}} view="secondary" label="Старт анализа" onClick={() => 
              {
                fetchCalculation()
              }} loading={btnLoad}/>
          </HeaderModule>
          <TabsExample x={x} value={tab}></TabsExample>
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
              { !btnLoad ? 
                <ComponentDemo data={repository?.hierarchyFilesJson} f={f}/> : <Loader></Loader>
              }
            </Card>
          </GridItem>
          </Grid>
        }
        { tab == "Результаты" &&
        <Grid gap="xl" cols={3}>
          { cardResult &&
            <GridItem col={1}>
                <Card verticalSpace="l" horizontalSpace="l" style={{height: '87vh', margin: '3vh 0 0 0'}}>
                { !btnLoad ?
                <div>
                  <Stats
                    value={result?.percentageLogs ?? 0}
                    title="Процент логов"
                    status="success" />
                  <Pie
                  style={{
                    width: 300,
                  }}
                  data={data}
                  angleField="value"
                  colorField="type"
                  />
                  <div id="resultTotal" >{result?.resultTotal}</div>
                </div> : <Loader></Loader>
                }
              </Card>
          </GridItem>
          }
          <GridItem col={cardResult ? 2 : 3}>
            <Card verticalSpace="l" horizontalSpace="l" style={{height: '87vh', margin: '3vh 3vh 0 0'}}>
              { !btnLoad ?
                <TableResult data={result?.resultJson} f={f}/> : <Loader></Loader>
              }
            </Card>
          </GridItem>
        </Grid>
        }
    </Theme>
  );
}

export default App;
