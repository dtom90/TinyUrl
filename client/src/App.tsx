import TinyUrlForm from './components/TinyUrlForm'
import TinyUrlList from './components/TinyUrlList'

function App() {
  return (
    <div className="min-h-screen flex flex-col items-center p-20 bg-gray-900 text-white">
      <h1 className="text-5xl font-bold">Tiny URL</h1>
      <TinyUrlForm />
      <TinyUrlList />
    </div>
  )
}

export default App
