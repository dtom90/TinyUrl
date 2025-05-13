import { useState } from "react"
import { apiClient } from "../lib/apiClient"

interface TinyUrlFormProps {
  onUrlCreated: () => void
}

export default function TinyUrlForm({ onUrlCreated }: TinyUrlFormProps) {
  const [inputUrl, setInputUrl] = useState('')

  const handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setInputUrl(event.target.value)
  }

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault()
    try {
      await apiClient.createTinyUrl(inputUrl)
      setInputUrl('')
      onUrlCreated()
    } catch (error) {
      console.error('Error creating tiny URL:', error)
    }
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
    </div>
  )
}
