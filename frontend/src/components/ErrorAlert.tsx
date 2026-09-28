import { Alert } from '@mui/material'
import { ApiError } from '../api/client'

/** Shows an API error and its field errors. */
export function ErrorAlert({ error }: { error: unknown }) {
  if (!error) return null

  const apiError = error instanceof ApiError ? error : new ApiError(0, 'Something went wrong. Please try again.')

  return (
    <Alert severity="error">
      {apiError.message}
      {apiError.errors.length > 0 && (
        <ul style={{ margin: '4px 0 0', paddingLeft: 18 }}>
          {apiError.errors.map((message) => <li key={message}>{message}</li>)}
        </ul>
      )}
    </Alert>
  )
}
