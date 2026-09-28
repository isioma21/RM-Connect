import { useCallback, useEffect, useState } from 'react'
import { CircularProgress, Stack } from '@mui/material'
import { relationshipsApi } from '../../api/relationships'
import type { Relationship } from '../../api/types'
import { ErrorAlert } from '../../components/ErrorAlert'
import { Greeting } from '../../components/Greeting'
import { ChooseManager } from './ChooseManager'
import { MyManager } from './MyManager'
import { PendingRequest } from './PendingRequest'

/** Shows the customer's manager, their pending request, or the list of managers to choose from. */
export function CustomerHomePage() {
  const [relationship, setRelationship] = useState<Relationship | null>()
  const [error, setError] = useState<unknown>(null)

  const load = useCallback(() => {
    relationshipsApi.current()
      .then((current) => setRelationship(current ?? null))
      .catch(setError)
  }, [])

  useEffect(load, [load])

  function content() {
    if (error) return <ErrorAlert error={error} />
    if (relationship === undefined) return <CircularProgress aria-label="Loading" />
    if (relationship === null) return <ChooseManager onChanged={load} />
    if (relationship.status === 'Pending') return <PendingRequest relationship={relationship} onChanged={load} />
    return <MyManager relationship={relationship} onChanged={load} />
  }

  return (
    <Stack spacing={3}>
      <Greeting />
      {content()}
    </Stack>
  )
}
