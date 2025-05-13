import { useQuery, useMutation, useQueryClient, QueryErrorResetBoundary } from "@tanstack/react-query"
import { apiClient, queryKeys } from "../lib/apiClient"
import ErrorMessage from "./ErrorMessage"

export default function TinyUrlList() {
  const queryClient = useQueryClient()

  const { data: tinyUrls = [], isLoading, error: listError } = useQuery({
    queryKey: [queryKeys.tinyUrls],
    queryFn: apiClient.listTinyUrls,
  })

  const { mutate: deleteTinyUrl, isPending: isDeleting, error: deleteError, reset: resetDelete } = useMutation({
    mutationFn: apiClient.deleteTinyUrl,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: [queryKeys.tinyUrls] })
    },
  })

  if (isLoading) {
    return <div>Loading...</div>
  }

  return (
    <div className="pt-8 w-full max-w-xl">
      <QueryErrorResetBoundary>
        <ErrorMessage error={listError || deleteError} onReset={() => resetDelete()} />

        <ul className="list-disc list-inside">
          {tinyUrls.map((url) => (
            <li key={url.id}>
              ID: {url.id} | Short URL: {url.shortUrl} | Long URL: {url.longUrl} | 
              <button 
                onClick={() => deleteTinyUrl(url.id)}
                disabled={isDeleting}
                className="text-red-500 hover:text-red-600 cursor-pointer disabled:opacity-50"
              >
                {isDeleting ? 'Deleting...' : 'Delete'}
              </button>
            </li>
          ))}
        </ul>
      </QueryErrorResetBoundary>
    </div>
  )
}
