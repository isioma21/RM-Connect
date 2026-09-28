import { useState } from 'react'
import { Alert, Box, Button, Card, CardContent, Stack, Typography } from '@mui/material'
import { relationshipsApi } from '../../api/relationships'
import type { Relationship } from '../../api/types'
import { ConfirmDialog } from '../../components/ConfirmDialog'
import { ErrorAlert } from '../../components/ErrorAlert'
import { StatusChip } from '../../components/StatusChip'
import { UserAvatar } from '../../components/UserAvatar'
import { formatDay, formatTime } from '../../utils/format'

type Props = { relationship: Relationship; onChanged: () => void }

export function PendingRequest({ relationship, onChanged }: Props) {
  const { manager } = relationship
  const [error, setError] = useState<unknown>(null)
  const [confirming, setConfirming] = useState(false)

  async function cancelRequest() {
    setConfirming(false)
    try {
      await relationshipsApi.end(relationship.id)
      onChanged()
    } catch (err) {
      setError(err)
    }
  }

  return (
    <Stack spacing={3} sx={{ maxWidth: 640 }}>
      <Typography variant="h1">My relationship manager</Typography>
      <ErrorAlert error={error} />
      <Card>
        <CardContent sx={{ p: 4 }}>
          <Stack spacing={3}>
            <Stack direction="row" spacing={2} sx={{ alignItems: 'center' }}>
              <UserAvatar user={manager} size={60} />
              <Box sx={{ flexGrow: 1 }}>
                <Typography sx={{ fontSize: 20, fontWeight: 700 }}>{manager.firstName} {manager.lastName}</Typography>
                <Typography color="text.secondary">{manager.branch} branch · {manager.email}</Typography>
              </Box>
              <StatusChip status="Pending" />
            </Stack>
            <Alert severity="warning" icon={false}>
              Waiting for {manager.firstName} to accept your request, sent {formatDay(relationship.requestedAt)} at {formatTime(relationship.requestedAt)}.
            </Alert>
            <Box>
              <Button variant="outlined" color="error" onClick={() => setConfirming(true)}>Cancel request</Button>
            </Box>
          </Stack>
        </CardContent>
      </Card>
      <ConfirmDialog
        open={confirming}
        title="Cancel request?"
        message={`Are you sure you want to cancel your request to ${manager.firstName}?`}
        confirmLabel="Yes, cancel request"
        onConfirm={cancelRequest}
        onClose={() => setConfirming(false)}
      />
    </Stack>
  )
}
