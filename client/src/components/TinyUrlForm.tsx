import { useState } from "react"

export default function TinyUrlForm() {
  const [inputUrl, setInputUrl] = useState('')
  const [longUrl, setLongUrl] = useState('')

  const handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setInputUrl(event.target.value)
  }

  const handleSubmit = (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault()
    setLongUrl(inputUrl)
    setInputUrl('')
  }

  return (
    <div className="p-8 w-full max-w-xl">
      <form className="flex flex-col gap-4" onSubmit={handleSubmit}>
        <label htmlFor="url">Shorten a long URL</label>
        <input type="text" id="url" value={inputUrl} onChange={handleChange} className="w-full px-6 py-3 rounded-lg bg-gray-800 border border-transparent text-base font-medium transition-colors hover:border-blue-500 focus:outline-none focus:ring-4 focus:ring-blue-500/40" />
        <button 
          type="submit"
          className="px-6 py-3 rounded-lg bg-gray-800 border border-transparent text-base font-medium transition-colors hover:border-blue-500 focus:outline-none focus:ring-4 focus:ring-blue-500/40"
        >
          Create
        </button>
      </form>
      {longUrl && (
        <p className="text-sm text-gray-400">
          Long URL: <a href={longUrl} target="_blank" rel="noopener noreferrer">{longUrl}</a>
        </p>
      )}
    </div>
  )
}
