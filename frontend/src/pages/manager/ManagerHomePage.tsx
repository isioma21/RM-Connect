import { useCallback, useEffect, useState } from 'react'
import { Stack } from '@mui/material'
import { relationshipsApi } from '../../api/relationships'
import type { Relationship } from '../../api/types'
import { ErrorAlert } from '../../components/ErrorAlert'
import { Greeting } from '../../components/Greeting'
import { MyCustomers } from './MyCustomers'
import { NewRequests } from './NewRequests'

/** The manager's new requests and active customers. */
export function ManagerHomePage() {
  const [relationships, setRelationships] = useState<Relationship[]>([])
  const [error, setError] = useState<unknown>(null)

  const load = useCallback(() => {
    relationshipsApi.forManager().then(setRelationships).catch(setError)
  }, [])

  useEffect(load, [load])

  return (
    <Stack spacing={4}>
      <Greeting />
      <ErrorAlert error={error} />
      <NewRequests requests={relationships.filter((r) => r.status === 'Pending')} onChanged={load} />
      <MyCustomers customers={relationships.filter((r) => r.status === 'Active')} onChanged={load} />
    </Stack>
  )
}
