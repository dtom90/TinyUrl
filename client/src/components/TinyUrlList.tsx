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

  const onDeleteClick = (id: string) => {
    if (confirm('Are you sure you want to delete this tiny URL?')) {
      deleteTinyUrl(id)
    }
  }

  return (
    <div className="pt-8 w-full max-w-xl">
      <QueryErrorResetBoundary>
        {listError || deleteError ? (
          <ErrorMessage error={listError || deleteError} onReset={() => resetDelete()} />
        ) : null}

        {isLoading ? (
          <div className="text-center text-gray-500">Loading...</div>
        ) : (
          tinyUrls.map((url) => (
            <div className="flex justify-between items-center border rounded-lg p-4 border-gray-700 pb-4 mb-4" key={url.id}>
            <span>
              <div>Short URL: <a href={url.shortUrl} target="_blank" rel="noopener noreferrer">{url.shortUrl}</a></div>
              <div>Long URL: <a href={url.longUrl} target="_blank" rel="noopener noreferrer">{url.longUrl}</a></div>
              <div>Click Count: {url.clickCount}</div>
            </span>
            <button 
              onClick={() => onDeleteClick(url.id)}
              disabled={isDeleting}
              className="delete-button"
            >
              {isDeleting ? 'Deleting...' : 'Delete'}
            </button>
            </div>
          ))
        )}
      </QueryErrorResetBoundary>
    </div>
  )
}
