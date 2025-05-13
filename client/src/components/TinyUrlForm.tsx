import { useState } from "react"
import { QueryErrorResetBoundary, useMutation, useQueryClient } from "@tanstack/react-query"
import ErrorMessage from "./ErrorMessage"
import { apiClient, queryKeys } from "../lib/apiClient"
import type { TinyUrlRequest } from "../types"

export default function TinyUrlForm() {
  const [inputUrl, setInputUrl] = useState('')
  const [customShortCode, setCustomShortCode] = useState('')
  const queryClient = useQueryClient()

  const createTinyUrlMutation = useMutation({
    mutationFn: (request: TinyUrlRequest) => apiClient.createTinyUrl(request),
    onSuccess: () => {
      setInputUrl('')
      setCustomShortCode('')
      queryClient.invalidateQueries({ queryKey: [queryKeys.tinyUrls] })
    },
  })

  const handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setInputUrl(event.target.value)
    createTinyUrlMutation.reset()
  }

  const handleCustomShortCodeChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setCustomShortCode(event.target.value)
    createTinyUrlMutation.reset()
  }

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault()
    createTinyUrlMutation.mutate({ longUrl: inputUrl, shortCode: customShortCode })
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
            placeholder="Enter a long URL"
            value={inputUrl}
            onChange={handleChange}
          />
          <div className="flex items-center gap-2">
            <span className="whitespace-nowrap">http://localhost:5226/</span>
            <input 
              type="text"
              id="url"
              placeholder="Custom short code (optional)"
              value={customShortCode}
              onChange={handleCustomShortCodeChange}
            />
          </div>
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
