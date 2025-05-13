interface ErrorMessageProps {
  error: Error | null;
  onReset: () => void;
}

export default function ErrorMessage({ error, onReset }: ErrorMessageProps) {

  return (
    <div className={`mb-4 p-4 rounded-lg bg-red-500/10 border border-red-500 text-red-500 transition-opacity duration-200 ${error ? 'opacity-100' : 'opacity-0 invisible'}`}>
      <div className="flex justify-between items-center">
        <span>{error?.message || 'no error'}</span>
        <button 
          onClick={onReset}
          className="ml-4 px-2 py-1 text-sm rounded bg-red-500/20 hover:bg-red-500/30 transition-colors"
        >
          Dismiss
        </button>
      </div>
    </div>
  )
}
