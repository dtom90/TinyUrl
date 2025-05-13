import { useState } from 'react'
import TinyUrlForm from './components/TinyUrlForm'
import TinyUrlList from './components/TinyUrlList'

function App() {
  const [refreshTrigger, setRefreshTrigger] = useState(0)

  const handleUrlCreated = () => {
    setRefreshTrigger(prev => prev + 1)
  }

  return (
    <div className="min-h-screen flex flex-col items-center justify-center p-8 bg-gray-900 text-white">
      <h1 className="text-5xl font-bold my-8">Tiny URL</h1>
      <TinyUrlForm onUrlCreated={handleUrlCreated} />
      <TinyUrlList refreshTrigger={refreshTrigger} />
    </div>
  )
}

export default App
