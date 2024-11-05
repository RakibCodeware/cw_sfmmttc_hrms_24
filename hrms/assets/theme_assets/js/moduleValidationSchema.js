const validationSchema = Yup.object().shape({
    moduleName: Yup.string().required('Module Name is required'),
    parentID: Yup.number().required('Parent ID is required').integer('Parent ID must be an integer'),
    url: Yup.string().url('Invalid URL format').required('Module URL is required'),
    physicalLocation: Yup.string().required('Physical Location is required'),
    iconClass: Yup.string().required('Icon Class is required'),
    ordering: Yup.number().required('Ordering is required').integer('Ordering must be an integer'),
    isActive: Yup.boolean().required('Status is required')
});

export default validationSchema;