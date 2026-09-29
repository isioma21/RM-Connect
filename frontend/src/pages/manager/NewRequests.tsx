import { useState } from 'react'
import { Box, Button, Card, Stack, Typography } from '@mui/material'
import { relationshipsApi } from '../../api/relationships'
import type { Relationship } from '../../api/types'
import { ConfirmDialog } from '../../components/ConfirmDialog'
import { ErrorAlert } from '../../components/ErrorAlert'
import { UserAvatar } from '../../components/UserAvatar'
import { formatDay, formatTime } from '../../utils/format'

type Props = { requests: Relationship[]; onChanged: () => void }

export function NewRequests({ requests, onChanged }: Props) {
  const [error, setError] = useState<unknown>(null)
  const [toDecline, setToDecline] = useState<Relationship | null>(null)

  async function accept(request: Relationship) {
    try {
      await relationshipsApi.accept(request.id)
      onChanged()
    } catch (err) {
      setError(err)
    }
  }

  async function decline() {
    if (!toDecline) return
    setToDecline(null)
    try {
      await relationshipsApi.decline(toDecline.id)
      onChanged()
    } catch (err) {
      setError(err)
    }
  }

  return (
    <Stack spacing={2}>
      <Typography variant="h2" component="h1">New requests ({requests.length})</Typography>
      <ErrorAlert error={error} />
      <Card>
        {requests.length === 0 && <Typography sx={{ p: 3 }} color="text.secondary">No new requests.</Typography>}
        {requests.map((request, index) => (
          <Stack
            key={request.id}
            direction={{ xs: 'column', sm: 'row' }}
            spacing={2}
            sx={{ px: { xs: 2, sm: 3 }, py: 2.5, alignItems: { xs: 'flex-start', sm: 'center' }, borderTop: index === 0 ? 0 : 1, borderColor: 'divider' }}
          >
            <UserAvatar user={request.customer} size={44} />
            <Box sx={{ flexGrow: 1 }}>
              <Typography sx={{ fontWeight: 700 }}>{request.customer.firstName} {request.customer.lastName}</Typography>
              <Typography variant="body2" color="text.secondary">
                {request.customer.email} · {request.customer.phone} · requested {formatDay(request.requestedAt)} at {formatTime(request.requestedAt)}
              </Typography>
            </Box>
            <Stack direction="row" spacing={1}>
              <Button variant="outlined" color="inherit" onClick={() => setToDecline(request)}>Decline</Button>
              <Button variant="contained" onClick={() => accept(request)}>Accept</Button>
            </Stack>
          </Stack>
        ))}
      </Card>
      <ConfirmDialog
        open={toDecline !== null}
        title="Decline request?"
        message={`Are you sure you want to decline ${toDecline?.customer.firstName}'s request?`}
        confirmLabel="Yes, decline"
        onConfirm={decline}
        onClose={() => setToDecline(null)}
      />
    </Stack>
  )
}
