function viewVisitorDetails(id) {
    const modal = new bootstrap.Modal(document.getElementById('visitorDetailModal'));
    modal.show();

    fetch(`/Visitor/Details?id=${id}`)
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            if (data.success) {
                const visitor = data.visitor;
                let html = `
                    <div class="row g-4">
                        <div class="col-12">
                            <div class="d-flex align-items-start gap-3">
                                <div class="rounded-circle d-flex align-items-center justify-content-center text-white fw-bold" style="width: 64px; height: 64px; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); font-size: 24px;">
                                    ${visitor.name.split(' ').map(n => n[0]).join('')}
                                </div>
                                <div class="flex-grow-1">
                                    <h3>${visitor.name}</h3>
                                    <div class="d-flex gap-2">
                                        <span class="badge badge-${visitor.status.toLowerCase()}">${visitor.status}</span>
                                        ${visitor.hasVehicle ? '<span class="badge bg-secondary"><i class="fas fa-car me-1"></i>Vehicle</span>' : ''}
                                        ${visitor.checkInTime && !visitor.checkOutTime ? '<span class="badge bg-info">Currently Inside</span>' : ''}
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="col-12"><hr/></div>

                        <div class="col-md-6">
                            <small class="text-muted">CNIC</small>
                            <p class="mb-0 fw-bold">${visitor.cnic}</p>
                        </div>
                        <div class="col-md-6">
                            <small class="text-muted">Phone</small>
                            <p class="mb-0 fw-bold">${visitor.phone}</p>
                        </div>
                        <div class="col-md-6">
                            <small class="text-muted">Host</small>
                            <p class="mb-0 fw-bold">${visitor.employeeName}</p>
                        </div>
                        <div class="col-md-6">
                            <small class="text-muted">Expected Time</small>
                            <p class="mb-0 fw-bold">${new Date(visitor.expectedTime).toLocaleString()}</p>
                        </div>
                        <div class="col-12">
                            <small class="text-muted">Purpose</small>
                            <p class="mb-0 fw-bold">${visitor.purpose}</p>
                        </div>
                `;

                if (visitor.hasVehicle) {
                    html += `
                        <div class="col-12"><hr/></div>
                        <div class="col-md-6">
                            <small class="text-muted">Vehicle Number</small>
                            <p class="mb-0 fw-bold">${visitor.vehicleNumber || 'N/A'}</p>
                        </div>
                        <div class="col-md-6">
                            <small class="text-muted">Vehicle Type</small>
                            <p class="mb-0 fw-bold">${visitor.vehicleType || 'N/A'}</p>
                        </div>
                    `;
                }

                if (visitor.checkInTime || visitor.checkOutTime) {
                    html += `<div class="col-12"><hr/></div>`;
                    if (visitor.checkInTime) {
                        html += `
                            <div class="col-md-6">
                                <small class="text-muted"><i class="fas fa-sign-in-alt me-1"></i>Check-in Time</small>
                                <p class="mb-0 fw-bold text-success">${new Date(visitor.checkInTime).toLocaleString()}</p>
                            </div>
                        `;
                    }
                    if (visitor.checkOutTime) {
                        html += `
                            <div class="col-md-6">
                                <small class="text-muted"><i class="fas fa-sign-out-alt me-1"></i>Check-out Time</small>
                                <p class="mb-0 fw-bold text-secondary">${new Date(visitor.checkOutTime).toLocaleString()}</p>
                            </div>
                        `;
                    }
                }

                if (visitor.approvedByName) {
                    html += `
                        <div class="col-12">
                            <div class="alert alert-success">
                                <strong><i class="fas fa-check-circle me-2"></i>Approved</strong><br/>
                                Approved by ${visitor.approvedByName} on ${visitor.approvedAt ? new Date(visitor.approvedAt).toLocaleString() : 'N/A'}
                            </div>
                        </div>
                    `;
                }

                if (visitor.rejectionReason) {
                    html += `
                        <div class="col-12">
                            <div class="alert alert-danger">
                                <strong><i class="fas fa-times-circle me-2"></i>Rejected</strong><br/>
                                Reason: ${visitor.rejectionReason}
                            </div>
                        </div>
                    `;
                }

                html += `</div>`;
                document.getElementById('modalContent').innerHTML = html;
            } else {
                document.getElementById('modalContent').innerHTML = `<div class="alert alert-danger"><i class="fas fa-exclamation-circle me-2"></i>${data.message || 'Unable to load visitor details'}</div>`;
            }
        })
        .catch(error => {
            console.error('Error fetching visitor details:', error);
            document.getElementById('modalContent').innerHTML = `
                <div class="alert alert-danger">
                    <i class="fas fa-exclamation-circle me-2"></i>
                    <strong>Error loading visitor details</strong><br/>
                    <small>Please try again. If the problem persists, contact support.</small>
                </div>
            `;
        });
}
