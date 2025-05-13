import TinyUrlForm from './components/TinyUrlForm'

function App() {
  return (
    <div className="min-h-screen flex flex-col items-center justify-center p-8 bg-gray-900 text-white">
      <h1 className="text-5xl font-bold my-8">Tiny URL</h1>
      <TinyUrlForm />
    </div>
  )
}

export default App
