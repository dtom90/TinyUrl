import { useState } from "react"
import { QueryErrorResetBoundary, useMutation, useQueryClient } from "@tanstack/react-query"
import { apiClient, queryKeys } from "../lib/apiClient"
import ErrorMessage from "./ErrorMessage"

export default function TinyUrlForm() {
  const [inputUrl, setInputUrl] = useState('')
  const queryClient = useQueryClient()

  const createTinyUrlMutation = useMutation({
    mutationFn: apiClient.createTinyUrl,
    onSuccess: () => {
      setInputUrl('')
      queryClient.invalidateQueries({ queryKey: [queryKeys.tinyUrls] })
    },
  })

  const handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setInputUrl(event.target.value)
    createTinyUrlMutation.reset()
  }

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault()
    createTinyUrlMutation.mutate(inputUrl)
  }

  return (
    <div className="pt-8 w-full max-w-xl">
      <QueryErrorResetBoundary>
        <ErrorMessage error={createTinyUrlMutation.error} onReset={() => createTinyUrlMutation.reset()} />
      
        <form className="flex flex-col gap-4" onSubmit={handleSubmit}>
          <label htmlFor="url">Shorten a long URL</label>
          <input 
            type="text" 
            id="url" 
            value={inputUrl} 
            onChange={handleChange} 
            className="w-full px-6 py-3 rounded-lg bg-gray-800 border border-transparent text-base font-medium transition-colors hover:border-blue-500 focus:outline-none focus:ring-4 focus:ring-blue-500/40" 
          />
          <button 
            type="submit"
            disabled={createTinyUrlMutation.isPending}
          >
            {createTinyUrlMutation.isPending ? 'Creating...' : 'Create'}
          </button>
        </form>
      </QueryErrorResetBoundary>
    </div>
  )
}
